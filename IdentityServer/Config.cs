using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using IdentityModel;
using System.Security.Claims;

namespace IsServerEfCore
{
    public static class Config
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            //Try adding claims to access token
            return new List<ApiResource>
            {
                //new ApiResource(
                //    "myapi",
                //    "My API",
                //    new[]
                //    {
                //        ClaimTypes.Name,
                //        ClaimTypes.Email,
                //        IdentityServerConstants.StandardScopes.OpenId,
                //        JwtClaimTypes.GivenName,
                //        JwtClaimTypes.FamilyName,
                //        IdentityServerConstants.StandardScopes.Email,
                //        "uid",
                //        JwtClaimTypes.Role,
                //        JwtClaimTypes.PhoneNumber,
                //        "testkey",
                //        JwtClaimTypes.ZoneInfo
                //    }

                //)
            };
        }


        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            //new IdentityResource
            //{
            //    Name = "role",
            //    UserClaims = new List<string> {"role", "username", "testkey"}
            //},
            // new IdentityResource
            //{
            //    Name = "myroles",
            //    UserClaims = new List<string> {JwtClaimTypes.PhoneNumber, JwtClaimTypes.Role, JwtClaimTypes.Email, JwtClaimTypes.Name, "testkey"}
            //},
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
           // new ApiScope("scope1", new List<string> {JwtClaimTypes.PhoneNumber, JwtClaimTypes.Role, JwtClaimTypes.Email, JwtClaimTypes.Name, "testkey"}),
            new ApiScope("scope", new List<string> {JwtClaimTypes.PhoneNumber, JwtClaimTypes.Role, JwtClaimTypes.Email, JwtClaimTypes.Name, "testkey"}),
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
            // m2m client credentials flow client
            new Client
            {
                ClientId = "m2m.client",
                ClientName = "Client Credentials Client",

                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("secret".Sha256()) },

                AllowedScopes = { "scope" }
            },

            // interactive client using code flow + pkce
            new Client
            {
                ClientId = "interactive",
                ClientSecrets = { new Secret("secret".Sha256()) },
                 AlwaysSendClientClaims = true,
                AlwaysIncludeUserClaimsInIdToken = true,
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
 RequireClientSecret = false,

                RedirectUris = { "https://localhost:7253/signin-oidc" },
                FrontChannelLogoutUri = "https://localhost:7253/signout-oidc",
                PostLogoutRedirectUris = { "https://localhost:7253/signout-callback-oidc" },

                AllowOfflineAccess = true,
                //AllowAccessTokensViaBrowser = true,
                AllowedScopes = { "openid", "myroles", "profile", "scope"}
            },
               new Client
            {
                ClientId = "interactive-1",
                ClientSecrets = { new Secret("secret".Sha256()) },
                AlwaysSendClientClaims = true,
                AlwaysIncludeUserClaimsInIdToken = true,
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
 RequireClientSecret = false,

                RedirectUris = { "https://localhost:7131/signin-oidc" },
                FrontChannelLogoutUri = "https://localhost:7131/signout-oidc",
                PostLogoutRedirectUris = { "https://localhost:7131/signout-callback-oidc" },

                AllowOfflineAccess = true,
                 //AllowAccessTokensViaBrowser = true,
                AllowedScopes = { "openid", "myroles",  "profile", "scope" }
            },


            };
    }
}