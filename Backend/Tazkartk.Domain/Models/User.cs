using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Tazkartk.Domain.Models
{
    public class User : Account
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? photo { get; set; }
        public ICollection<Booking> books { get; set; } = new List<Booking>();


    }
}
