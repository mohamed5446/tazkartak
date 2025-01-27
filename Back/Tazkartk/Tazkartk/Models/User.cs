using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Tazkartk.Models
{
    public class User : IdentityUser<int>
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        [RegularExpression(@"^0[0-9]{10}$", ErrorMessage = "Invalid Phone Number")]
        public string PhoneNumber { get; set; }
        public string? photo { get; set; }
        public List<Booking>? books { get; set; }

    }
}
