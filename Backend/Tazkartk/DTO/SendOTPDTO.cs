using System.ComponentModel.DataAnnotations;

namespace Tazkartk.DTO
{
    public class SendOTPDTO
    {
        [Required,EmailAddress]
        public string Email {  get; set; }
    }
}
