﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tazkartk.Domain.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int? UserId { get; set; }
        public int PaymentId { get; set; }
        public bool IsCanceled { get; set; }
        public int tripId { get; set; }
        public string? GuestFirstName { get; set; }
        public string? GuestLastName { get; set; }
        public string? GuestPhoneNumber { get; set; }
        public Trip trip { get; set; }
        public User? user { get; set; }
        public Payment payment { get; set; }
        public List<Seat> seats { get; set; } = new List<Seat>();
    }
}
