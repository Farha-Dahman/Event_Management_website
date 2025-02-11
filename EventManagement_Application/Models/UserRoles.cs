using System.IO;

namespace EventManagement_Application.Models
{
    public class UserRoles
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<Role> Roles { get; set; }
    }
}
