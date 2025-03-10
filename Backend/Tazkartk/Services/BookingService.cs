using Microsoft.EntityFrameworkCore;
using Tazkartk.DTO;
using Tazkartk.DTO.UserDTOs;
using Tazkartk.Interfaces;
using Tazkartk.Models.Enums;
using Tazkartk.Models;
using Tazkartk.Data;
using Tazkartk.DTO.Response;

namespace Tazkartk.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPaymobService _paymobService;
        public BookingService(ApplicationDbContext context , IPaymobService paymobService)
        {
            _context = context;
            _paymobService = paymobService;
        }
        #region Booking
        public async Task<string?> BookSeat(BookingDTO DTO)
        {
            var user = await _context.Users.FindAsync(DTO.UserId);
            var trip = await _context.Trips.Include(t => t.seats).FirstOrDefaultAsync(t => t.TripId == DTO.TripId);
            if (trip == null || user == null) { return null; }
            if (trip.seats == null)
            {
                trip.seats = new List<Seat>();
            }
            var UserDetailsDTO = new UserDetailsDTO
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber
            };
            var isbooked = DTO.SeatsNumbers.Any(number => trip.seats.Any(s => s.Number == number));

            if (isbooked)
            {
                return null;
            }
            int count = DTO.SeatsNumbers.Count;
            double total = trip.Price * count;
            var Url = await _paymobService.CreatePaymentIntent(DTO, total, UserDetailsDTO);
            if (!string.IsNullOrEmpty(Url)) return Url;
            return null;
        }

        public async Task<bool> ConfirmBooking(BookingDTO DTO, string PaymentIntentId, string PaymentMethod)
        {
            var user = await _context.Users.Include(u => u.books).FirstOrDefaultAsync(u => u.Id == DTO.UserId);
            var trip = await _context.Trips.Include(t => t.seats).FirstOrDefaultAsync(t => t.TripId == DTO.TripId);
            if (user == null || trip == null) return false;
            var booking = new Booking
            {
                trip = trip,
                tripId = trip.TripId,
                user = user,
                UserId = user.Id,
                IsCanceled = false,
                seats = new List<Seat>()
            };

            await _context.bookings.AddAsync(booking);
            await _context.SaveChangesAsync();

            var seats = DTO.SeatsNumbers.Select(seatNumber => new Seat
            {
                Number = seatNumber,
                State = SeatState.Booked,
                booking = booking,
                trip = trip,
                bookingId = booking.BookingId,
                TripId = trip.TripId
            }).ToList();

            booking.seats.AddRange(seats);
            await _context.SaveChangesAsync();
            var payment = new Payment
            {
                Method = PaymentMethod,
                Date = DateTime.UtcNow,
                amount = trip.Price * DTO.SeatsNumbers.Count,
                bookingId = booking.BookingId,
                IsRefunded = false,
                booking = booking,
                PaymentIntentId = PaymentIntentId
            };


            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            booking.payment = payment;
            booking.PaymentId = payment.PaymentId;
            await _context.SaveChangesAsync();

            return true;
        }
        #endregion

        #region Cancel
        public async Task<bool> Refund(int bookingId)
        {
            var booking = await _context.bookings.Include(b => b.seats).Include(b => b.trip).Include(b => b.payment).FirstOrDefaultAsync(b => b.BookingId == bookingId);
            if (booking == null||booking.IsCanceled) return false;
            var count = booking.seats.Count();
            var total = (count * booking.trip.Price) * 100;
            var paymemnt = await _context.Payments.FirstOrDefaultAsync(p => p.bookingId == bookingId);
            if (paymemnt == null||paymemnt.IsRefunded) return false;
            var trxId = paymemnt.PaymentIntentId;
            var success = await _paymobService.RefundTransaction(bookingId, trxId, total);
            if (!success) return false;
            return true;
        }
        public async Task<bool> Cancel(string trxId)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.PaymentIntentId == trxId);
            if (payment == null) return false;
            var paymentId = payment.PaymentId;
            var booking = await _context.bookings.Include(b => b.seats).Include(b => b.trip).FirstOrDefaultAsync(b => b.PaymentId == paymentId);
            if (booking == null) return false;
            var count = booking.seats.Count;
            var total = (count * booking.trip.Price) * 100;

            foreach (var seat in booking.seats)
            {
                seat.State = SeatState.Available;
            }
            payment.IsRefunded = true;
            booking.IsCanceled = true;
            await _context.SaveChangesAsync();
            return true;
        }
        #endregion


      
        

        public async Task<bool> DeleteBooking(int Id)
        {
            var booking = await _context.bookings.FindAsync(Id);
            if (booking == null)
            {
                return false;
            }

            _context.bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return true;
        }

       

        public async Task<List<TicketDTO>?> GetBookings()
        {
            return await _context.bookings
               .AsNoTracking()
               .Select(b => new TicketDTO
               {
                   UserId = b.user.Id,
                   userEmail = b.user.Email,
                   CompanyName = b.trip.company.Name,
                   From = b.trip.From,
                   To = b.trip.To,
                   Date = b.trip.Date.ToString("dddd yyyy-MM-dd"),
                   Time = b.trip.Time.ToString("HH:mm tt"),
                   BookingId = b.BookingId,
                   Name = b.user.FirstName,
                   IsCanceled = b.IsCanceled,
                   SeatsNumbers = b.seats.Select(s => s.Number).ToList(),
               }).ToListAsync();
        }

        

        public async Task<List<TicketDTO>?> GetUserBookings(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return null;
            return await _context.bookings.Where(b => b.UserId == userId).AsNoTracking()
               .Select(b => new TicketDTO
               {
                   UserId = b.user.Id,
                   userEmail = b.user.Email,
                   CompanyName = b.trip.company.Name,
                   From = b.trip.From,
                   To = b.trip.To,
                   Date = b.trip.Date.ToString("dddd yyyy-MM-dd"),
                   Time = b.trip.Time.ToString("HH:mm tt"),
                   BookingId = b.BookingId,
                   Name = b.user.FirstName,
                   IsCanceled = b.IsCanceled,
                   SeatsNumbers = b.seats.Select(s => s.Number).ToList(),
               }).ToListAsync();
        }
       
        
    }
}
