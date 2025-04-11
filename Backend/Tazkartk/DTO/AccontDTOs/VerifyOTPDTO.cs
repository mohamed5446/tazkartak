using System.ComponentModel.DataAnnotations;

namespace Tazkartk.DTO.AccontDTOs
{
    public class VerifyOTPDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required, RegularExpression(@"^\d{6}$", ErrorMessage = "رمز التحقق يجب أن يكون مكونًا من 6 أرقام")]
        public string EnteredOtp { get; set; }
    }
}
