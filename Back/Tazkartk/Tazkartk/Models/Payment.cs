using System.ComponentModel.DataAnnotations;

namespace Tazkartk.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public string Method { get; set; }
        public DateTime Date { get; set; }
        public int bookingId { get; set; }
        public bool IsRefunded { get; set; } = false;
        public Booking booking { get; set; }

    }
}
