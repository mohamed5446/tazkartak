using Microsoft.AspNetCore.Identity;

namespace Tazkartk.Models
{
    public class Company:Account
    {
        public string Name { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string? Logo { get; set; }
        public double Balance {  get; set; }
        public List<Trip>? Trips { get; set; }
    }
}
