using System.ComponentModel.DataAnnotations;

namespace Tazkartk.DTO.AccontDTOs
{
    public class RegisterDTO
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required, RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "Invalid Phone Number")]
        public string PhoneNumber { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
