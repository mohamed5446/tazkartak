using System.ComponentModel.DataAnnotations;

namespace Tazkartk.Application.DTO.AccontDTOs
{
    public class SendOTPDTO
    {
        [Required, EmailAddress(ErrorMessage = "البريد الإلكتروني غير صالح"), RegularExpression(@"^[^@]+@[^@]+\.[^@]+$", ErrorMessage = "يرجى إدخال رقم هاتف صحيح")]
        public string Email { get; set; }
    }
}
