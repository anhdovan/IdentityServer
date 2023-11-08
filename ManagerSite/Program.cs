using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Site2
{

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("require-manager-role", options =>
                {
                    options.RequireClaim(JwtClaimTypes.Role, "admin", "manager");
                });
            });
            //JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
                .AddCookie("Cookies", options => {
                    options.AccessDeniedPath = "/account/accessdenied";
            })
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = "https://localhost:5001";

                    options.ClientId = "interactive-1";
                    options.UsePkce = true;
                    options.ClientSecret = "secret";
                    options.ResponseType = "code";
                    options.ResponseMode = "form_post";
                    //options.Prompt = "consent";
                    options.MapInboundClaims = false;
                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.ClaimActions.MapUniqueJsonKey(JwtClaimTypes.PhoneNumber, "phone_number");
                    options.ClaimActions.MapUniqueJsonKey(JwtClaimTypes.Name, "name");
                    options.ClaimActions.MapUniqueJsonKey(JwtClaimTypes.Role, JwtClaimTypes.Role);
                    options.MapInboundClaims = false;

                   options.Scope.Add("scope");
                    options.Scope.Add("offline_access");

                    options.ClaimActions.MapAllExcept("iss", "nbf", "exp", "aud", "nonce", "iat", "c_hash");
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role"
                    };
                });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                  .RequireAuthorization("require-manager-role");

            app.Run();
        }
    }


}