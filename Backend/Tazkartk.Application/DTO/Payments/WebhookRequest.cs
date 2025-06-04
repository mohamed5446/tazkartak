using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazkartk.Application.DTO.Payments
{
    public class WebhookRequest
    {

        public bool IsSuccessful { get; set; }
        public bool IsRefunded { get; set; }
        public string TransactionId { get; set; }
        public string PaymentMethod { get; set; }
        //public int UserId { get; set; }
        //public int TripId { get; set; }
        //public List<int> Seats { get; set; }
        public BookingDTO extra { get; set; }

    }
}
