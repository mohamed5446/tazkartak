using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Tazkartk.Application.DTO.CompanyDTOs
{
    public class CompanyEditDTO
    {
        public string? Name { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public IFormFile? Logo { get; set; }
        [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "يرجى إدخال رقم هاتف صحيح")]

        public string? PhoneNumber { get; set; }
    }
}
