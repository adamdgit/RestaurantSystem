using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BitByByte.Data
{
    public class ApplicationUser:IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string ProfileUrl { get; set; }
        public string Role { get; set; }
    }
}
