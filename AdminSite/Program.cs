using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace LoginSite
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
                options.AddPolicy("require-admin-role", options => options.RequireClaim(JwtClaimTypes.Role, "admin"));
            });
            //JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";

            })
                .AddCookie("Cookies", options =>
                {
                    options.AccessDeniedPath = "/account/accessdenied";
                    options.Events.OnSigningIn = ctx =>
                    {
                        Console.WriteLine();
                        Console.WriteLine("Claims received by the Cookie handler");
                        foreach (var claim in ctx.Principal.Claims)
                        {
                            Console.WriteLine($"{claim.Type} - {claim.Value}");
                        }
                        Console.WriteLine();

                        return Task.CompletedTask;
                    };
                })
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = "https://localhost:5001";

                    options.ClientId = "interactive";
                    options.UsePkce = true;
                    options.ClientSecret = "secret";
                    options.ResponseType = "code";
                    options.ResponseMode = "form_post";
                    //options.Prompt = "consent";
                    options.MapInboundClaims = false;
                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.Scope.Add("scope");

                    options.ClaimActions.MapAllExcept("iss", "nbf", "exp", "aud", "nonce", "iat", "c_hash");
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role"
                    };

                    options.Events.OnUserInformationReceived = ctx =>
                    {
                        Console.WriteLine();
                        Console.WriteLine("Claims from the ID token");
                        foreach (var claim in ctx.Principal.Claims)
                        {
                            Console.WriteLine($"{claim.Type} - {claim.Value}");
                        }
                        Console.WriteLine();
                        Console.WriteLine("Claims from the UserInfo endpoint");
                        foreach (var property in ctx.User.RootElement.EnumerateObject())
                        {
                            Console.WriteLine($"{property.Name} - {property.Value}");
                        }
                        return Task.CompletedTask;
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
                  .RequireAuthorization("require-admin-role");

            app.Run();
        }
    }

}