using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tazkartk.Models.Enums;

namespace Tazkartk.Models
{
    public class Seat
    {
        public int SeatId { get; set; }
        public int TripId { get; set; }
        public int Number { get; set; }
        public SeatState State { get; set; }=SeatState.Available;
        public int? bookingId { get; set; }
        public Booking? booking { get; set; }
        public Trip trip { get; set; }
    }
}
