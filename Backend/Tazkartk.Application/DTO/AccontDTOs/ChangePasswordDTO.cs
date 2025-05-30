using System.ComponentModel.DataAnnotations;

namespace Tazkartk.Application.DTO.AccontDTOs
{
    public class ChangePasswordDTO
    {
        [Required, EmailAddress(ErrorMessage = "البريد الإلكتروني غير صالح")/*, RegularExpression(@"^[^@]+@[^@]+\\.[^@]+$", ErrorMessage = "يرجى إدخال رقم هاتف صحيح")*/]
        public string Email { get; set; }
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string newPassword { get; set; }
    }
}
