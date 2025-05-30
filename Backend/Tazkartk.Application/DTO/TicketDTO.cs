using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Tazkartk.Application.DTO
{
    public class TicketDTO
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        [Required]
        public string Name { get; set; }
        public string userEmail { get; set; }
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public string DepartureDate { get; set; }
        [Required]
        public string DepartureTime { get; set; }
        [Required]
        public string DepartureDay { get; set; }

        //  public Date DepartureTime { get; set; }
        //public string Date {  get; set; }
        //public string Time {  get; set; }
        [Required]
        public string From { get; set; }
        [Required]
        public string To { get; set; }
        public bool IsCanceled { get; set; }
        public List<int> SeatsNumbers { get; set; }
        public string CompanyLogoUrl { get; set; }


    }
}
