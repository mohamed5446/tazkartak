using System.ComponentModel.DataAnnotations;

namespace Tazkartk.DTO.AccontDTOs
{
    public class SendOTPDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
