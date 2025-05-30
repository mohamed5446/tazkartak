using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Tazkartk.Application.DTO.UserDTOs
{
    public class EditUserDTO
    {
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "يرجى إدخال رقم هاتف صحيح")]
        public string? Phone { get; set; }

        public IFormFile? photo { get; set; }
    }
}
