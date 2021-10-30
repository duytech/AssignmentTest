using Microsoft.AspNetCore.Identity;

namespace Core_3._1.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }
    }
}
