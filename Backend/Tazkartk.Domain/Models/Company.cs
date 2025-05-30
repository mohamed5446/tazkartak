using Microsoft.AspNetCore.Identity;

namespace Tazkartk.Domain.Models
{
    public class Company : Account
    {
        public string Name { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string? Logo { get; set; }
        public double Balance { get; set; }
        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}
