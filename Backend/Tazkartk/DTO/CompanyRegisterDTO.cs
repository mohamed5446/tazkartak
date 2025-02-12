using System.ComponentModel.DataAnnotations;

namespace Tazkartk.DTO
{
    public class CompanyRegisterDTO
    {
        [Required]
        public string Name { get; set; }
        [Required,EmailAddress]
        public string Email { get; set; }
        [Required, RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "Invalid Phone Number")]
        public string Phone { get; set; }
        [Required]
        public string Password { get; set; }
        public string city { get; set; }
        public string street { get; set; }
    }
}
