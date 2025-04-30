using System.ComponentModel.DataAnnotations;

namespace Tazkartk.DTO.CompanyDTOs
{
    public class CompanyRegisterDTO
    {
        [Required]
        public string Name { get; set; }
        [Required, EmailAddress(ErrorMessage ="البريد الإلكتروني غير صالح")]
        public string Email { get; set; }
        [Required, RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "يرجى إدخال رقم هاتف صحيح")]
        public string Phone { get; set; }
        [Required]
        public string Password { get; set; }
        public string city { get; set; }
        public string street { get; set; }
    }
}
