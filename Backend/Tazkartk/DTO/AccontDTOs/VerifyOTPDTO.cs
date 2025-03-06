using System.ComponentModel.DataAnnotations;

namespace Tazkartk.DTO.AccontDTOs
{
    public class VerifyOTPDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required, RegularExpression(@"^\d{6}$", ErrorMessage = "OTP is 6 digits ")]
        public string EnteredOtp { get; set; }
    }
}
