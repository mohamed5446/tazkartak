using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazkartk.Application.DTO.Payments
{
    public class UserBookingDetails
    {
        public int userId { get; set; }
        public int tripId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
        public List<int> seatNumbers { get; set; }
       
      //  int userId, int tripId, List<int> seatNumbers, double amount, string firstname, string lastname, string PhoneNumber, string Email
    }
}
