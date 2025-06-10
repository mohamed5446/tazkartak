using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tazkartk.Domain.Models.Enums;

namespace Tazkartk.Domain.Models
{
    public class Seat
    {
        public int TripId { get; set; }
        public int Number { get; set; }
        public SeatState State { get; set; } = SeatState.Available;
        public int? bookingId { get; set; }
        public Booking? booking { get; set; }
        public Trip trip { get; set; }
    }
}
