using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using System.Security.Claims;

namespace Adv.IdentityServer
{
    public class MyProfileService : IProfileService
    {
        public MyProfileService()
        {
        }
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var username = context.Subject.Identity.Name;
            var user = SimpleMembership.Membership.GetUser(username);
            var claims = new List<Claim>()
        {

             new Claim (JwtClaimTypes.Name, user.Username),
              new Claim ("fullname", user.Name)
        };
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, role.Name));
            }
            context.IssuedClaims.AddRange(claims);
            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            return Task.CompletedTask;
        }
    }
}
