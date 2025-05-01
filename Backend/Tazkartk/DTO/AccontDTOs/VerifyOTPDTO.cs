using System.ComponentModel.DataAnnotations;

namespace Tazkartk.DTO.AccontDTOs
{
    public class VerifyOTPDTO
    {
        [Required, EmailAddress(ErrorMessage = "البريد الإلكتروني غير صالح"), RegularExpression(@"^[^@]+@[^@]+\.[^@]+$", ErrorMessage = "يرجى إدخال رقم هاتف صحيح")]
        public string Email { get; set; }
        [Required, RegularExpression(@"^\d{6}$", ErrorMessage = "رمز التحقق يجب أن يكون مكونًا من 6 أرقام")]
        public string EnteredOtp { get; set; }
    }
}
