using System.ComponentModel.DataAnnotations;

namespace Tazkartk.Domain.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public string PaymentIntentId { get; set; }
        public string Method { get; set; }
        public double amount { get; set; }
        public DateTime Date { get; set; }
        public int bookingId { get; set; }
        public bool IsRefunded { get; set; } = false;
        public Booking booking { get; set; }

    }
}
