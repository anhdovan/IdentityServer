using System.Security.Claims;

namespace SimpleMembership
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public List<Role> Roles { get; set; } = new List<Role>();
        public string SubjectId { get; set; }
        public string Name { get; set; }
    }
}