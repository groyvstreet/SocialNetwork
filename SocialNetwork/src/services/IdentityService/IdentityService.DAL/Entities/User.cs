using Microsoft.AspNetCore.Identity;

namespace IdentityService.DAL.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public DateTime BirthDate { get; set; }
    }
}
