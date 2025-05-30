using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tazkartk.Domain.Models
{
    public class Trip
    {
        public int TripId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public bool Avaliblility { get; set; } = true;
        public string Class { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        // public DateTime ArriveTime { get; set; }
        public string Location { get; set; }
        public double Price { get; set; }
        public int TripCode { get; set; }
        public int CompanyId { get; set; }
        public Company company { get; set; }
        public List<Seat> seats { get; set; }
        public ICollection<Booking> bookings { get; set; } = new List<Booking>();

    }
}
