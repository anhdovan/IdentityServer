using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMembership
{
    public class Membership
    {
        private static Role AdminRole = new Role { Name = "admin" };
        private static Role ManagerRole = new Role { Name = "manager" };
        private static Role UsernRole = new Role { Name = "user" };

        private static List<User> _users = new List<User>
        {
            new User
            {
                Name = "Quản trị viên",
                Username = "admin",
                Password = "admin",
                SubjectId = "1",
                Roles = new List<Role> {AdminRole}
            },
             new User
            {
                 Name = "Quản lý",
                Username = "manager",
                Password = "manager",
                 SubjectId = "2",
                Roles = new List<Role> {ManagerRole}
            },
              new User
            {
                  Name = "Người dùng",
                Username = "user",
                Password = "user",
                 SubjectId = "3",
                Roles = new List<Role> {UsernRole}
            }
        };

        public static bool Login(string username, string password)
        {
            return _users.Any(u => u.Username == username && u.Password == password);
        }
        public static User GetUser(string username)
        {
            return _users.FirstOrDefault(u => u.Username == username);
        }

        public static List<string> GetUserRoles(string username)
        {
            return _users.FirstOrDefault(u => u.Username == username).Roles.Select(r => r.Name).ToList();
        }
    }
}
