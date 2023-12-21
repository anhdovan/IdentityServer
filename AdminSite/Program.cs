using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Serilog;
using System.Net;
namespace LoginSite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, e) => true;
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseSerilog((ctx, lc) => lc
     //.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
     //.WriteTo.File("serilog.txt")
     .Enrich.FromLogContext()
     .ReadFrom.Configuration(ctx.Configuration));
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
                    HttpClientHandler handler = new HttpClientHandler();
                    handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                    options.BackchannelHttpHandler = handler;

                    var iscfg = builder.Configuration.GetSection("IsConfig");
                    var serverUrl = iscfg.GetValue("".GetType(), "url").ToString();
                    var clientId = iscfg.GetValue("".GetType(), "clientId").ToString();
                    var clientSecret = iscfg.GetValue("".GetType(), "clientSecret").ToString();
                    options.Authority = serverUrl;

                    options.ClientId = clientId;
                    options.UsePkce = true;
                    options.ClientSecret = clientSecret;
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
            app.UseSerilogRequestLogging();
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