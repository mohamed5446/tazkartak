using System.ComponentModel.DataAnnotations;

namespace Tazkartk.Application.DTO.Google
{
    public class GooglesigninDTO
    {
        [Required]
        public string IdToken { get; set; }
    }
}
