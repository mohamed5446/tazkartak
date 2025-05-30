using System.ComponentModel.DataAnnotations;

namespace Tazkartk.Application.DTO.AccontDTOs
{
    public class LoginDTO
    {
        [Required, EmailAddress(ErrorMessage = "البريد الإلكتروني غير صالح")/*, RegularExpression(@"^[^@]+@[^@]+\\.[^@]+$", ErrorMessage = "يرجى إدخال رقم هاتف صحيح")*/]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
