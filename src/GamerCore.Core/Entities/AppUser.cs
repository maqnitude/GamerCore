using Microsoft.AspNetCore.Identity;

namespace GamerCore.Core.Entities
{
    public class AppUser : IdentityUser
    {
        public AppUser()
        {
        }

        public AppUser(string userName) : base(userName)
        {
        }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}