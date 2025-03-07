using System.ComponentModel.DataAnnotations;

namespace Tazkartk.DTO.UserDTOs
{
    public class EditUserDTO
    {
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        [ RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "Invalid Phone Number")]
        public string? Phone { get; set; }

        public IFormFile? photo { get; set; }
    }
}
