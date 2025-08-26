using Microsoft.AspNetCore.Identity;

namespace Demo.DataAccess.Models.IdentityModel
{
    public class ApplicationUser:IdentityUser
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime RegisteredAt { get; set; }
    }
}
