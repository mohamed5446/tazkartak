using System.ComponentModel.DataAnnotations;

namespace Tazkartk.Models
{
    public class Company
    {
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        [RegularExpression(@"^0[0-9]{10}$", ErrorMessage = "Invalid Phone Number")]
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string? Logo { get; set; }
        public List<Trip>? Trips { get; set; }

    }
}
