using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Tazkartk.Models
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? photo { get; set; }
        public List<Booking>? books { get; set; }

    }
}
