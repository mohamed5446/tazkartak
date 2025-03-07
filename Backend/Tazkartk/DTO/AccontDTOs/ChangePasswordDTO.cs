using System.ComponentModel.DataAnnotations;

namespace Tazkartk.DTO.AccontDTOs
{
    public class ChangePasswordDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string newPassword { get; set; }
    }
}
