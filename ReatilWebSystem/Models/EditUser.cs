using System.ComponentModel.DataAnnotations;

namespace BitByByte.Models
{
    public class EditUser
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        //[Required]
        public string Role { get; set; }
        public string ProfileUrl { get; set; }
    }
}
