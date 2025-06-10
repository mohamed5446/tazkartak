using System.ComponentModel.DataAnnotations;

namespace Tazkartk.Application.DTO.AccontDTOs
{
    public class ChangePasswordDTO
    {
        [Required, EmailAddress(ErrorMessage = "البريد الإلكتروني غير صالح"), RegularExpression(@"^[^@]+@[^@]+\.[^@]+$", ErrorMessage = "البريد الإلكتروني غير صالح")]
        public string Email { get; set; }
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string newPassword { get; set; }
    }
}
