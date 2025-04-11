using System.ComponentModel.DataAnnotations;

namespace Tazkartk.DTO.AccontDTOs
{
    public class ResetPasswordDTO
    {
        [Required, EmailAddress]
        public string email { get; set; }
        [Required, RegularExpression(@"^\d{6}$", ErrorMessage = "رمز التحقق يجب أن يكون مكونًا من 6 أرقام ")]

        public string OTP { get; set; }
        [Required]

        public string newPasswod { get; set; }
    }
}