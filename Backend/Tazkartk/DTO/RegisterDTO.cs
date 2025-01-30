using System.ComponentModel.DataAnnotations;

namespace Tazkartk.DTO
{
    public class RegisterDTO
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required,EmailAddress]
        public string Email { get; set; }
        [Required,RegularExpression(@"^0[0-9]{10}$", ErrorMessage = "Invalid Phone Number")]

        public string PhoneNumber { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
