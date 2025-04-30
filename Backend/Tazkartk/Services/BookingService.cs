using Microsoft.EntityFrameworkCore;
using Tazkartk.DTO;
using Tazkartk.DTO.UserDTOs;
using Tazkartk.Interfaces;
using Tazkartk.Models.Enums;
using Tazkartk.Models;
using Tazkartk.Data;
using Tazkartk.DTO.Response;
using System.Globalization;
using Tazkartk.DTO.CompanyDTOs;
using AutoMapper.QueryableExtensions;
using AutoMapper;

namespace Tazkartk.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPaymobService _paymobService;
        private readonly IMapper _mapper;

        public BookingService(ApplicationDbContext context, IPaymobService paymobService, IMapper mapper)
        {
            _context = context;
            _paymobService = paymobService;
            _mapper = mapper;
        }
        #region Booking
        public async Task<ApiResponse<string>> BookSeatAsync(BookingDTO DTO)
        {
            var user = await _context.Users.FindAsync(DTO.UserId);
            var trip = await _context.Trips.Include(t => t.seats).FirstOrDefaultAsync(t => t.TripId == DTO.TripId);
            if (trip == null || user == null || trip.Avaliblility == false)
            {
                return ApiResponse<string>.Error("حدث خطا");
            }
            if (trip.seats == null)
            {
                trip.seats = new List<Seat>();
            }
            var UserDetailsDTO = _mapper.Map<UserDetailsDTO>(user);

            var isbooked = DTO.SeatsNumbers.Any(number => trip.seats.Any(s => s.Number == number && s.State == SeatState.Booked));

            if (isbooked)
            {
                return ApiResponse<string>.Error("بعض المقاعد التي اخترتها محجوزة بالفعل");
            }
            int count = DTO.SeatsNumbers.Count;
            double total = trip.Price * count;
            string Url = await _paymobService.CreatePaymentIntentAsync(DTO, total, UserDetailsDTO);

            if (!string.IsNullOrEmpty(Url))
            {
                return ApiResponse<string>.success("payment url", Url);
            }
            return ApiResponse<string>.Error("حدث خطا");
        }
        public async Task<bool> ConfirmBookingAsync(BookingDTO DTO, string PaymentIntentId, string PaymentMethod)
        {
            var user = await _context.Users.Include(u => u.books).FirstOrDefaultAsync(u => u.Id == DTO.UserId);
            var trip = await _context.Trips.Include(t => t.seats).FirstOrDefaultAsync(t => t.TripId == DTO.TripId);
            
            if (user == null || trip == null) return false;
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
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

                await transaction.CommitAsync();

                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return false;
            }

        }


           
        #endregion

        #region Cancel
        public async Task<ApiResponse<bool>> RefundAsync(int bookingId)
        {
           
            var booking = await _context.bookings.Include(b => b.seats).Include(b => b.trip).Include(b => b.payment).FirstOrDefaultAsync(b => b.BookingId == bookingId);
            if (booking == null)
            {
                return ApiResponse<bool>.Error("حدث خطا");
            }
            var DepartureTime = booking.trip.Date.ToDateTime(booking.trip.Time);
            if(DateTime.Now>DepartureTime.AddHours(-2))
            {
                return ApiResponse<bool>.Error("لا يمكن الغاء الرحلة باقي اقل من ساعتان");
            }
            if (booking.IsCanceled==true)
            {
                return ApiResponse<bool>.Error("لقد قمت بالغاء الحجز");
            }
            var count = booking.seats.Count();
            var total = (count * booking.trip.Price) * 100;
            var paymemnt = await _context.Payments.FirstOrDefaultAsync(p => p.bookingId == bookingId);
            if(paymemnt.Method=="wallet")
            {
                ApiResponse<bool>.Error("الاسترداد عبر المحفظة غير متاح في الوقت الحالي");
            }
            if (paymemnt == null || paymemnt.IsRefunded)
            {
                return ApiResponse<bool>.Error("حدث خطا");
            }
            var trxId = paymemnt.PaymentIntentId;
              var result =  await _paymobService.RefundTransactionAsync(bookingId, trxId, total);
            if (!result) return ApiResponse<bool>.Error("حدث خطا");
                    return ApiResponse<bool>.success("تم تحويل المبلغ . يرجى التأكد من حسابك، وإذا واجهت أي مشكلة،برجاء التواصل معنا. ");
        }
        public async Task<bool> CancelAsync(string trxId)
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

        public async Task<ApiResponse<string>> DeleteBookingAsync(int Id)
        {
                var booking = await _context.bookings.Include(b => b.seats).FirstOrDefaultAsync(b => b.BookingId == Id);
                var payment = await _context.Payments.FindAsync(booking.PaymentId);
                _context.Seats.RemoveRange(booking.seats);
                _context.Payments.Remove(payment);
                _context.bookings.Remove(booking);

                await _context.SaveChangesAsync();
                return ApiResponse<string>.success("تم حذف الحجز");
        }



        public async Task<List<TicketDTO>> GetBookingsAsync()
        {
            return await _context.bookings
                .AsNoTracking()
                .ProjectTo<TicketDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }



        public async Task<List<TicketDTO>> GetUserBookingsAsync(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u=>u.Id==userId);
            if (user == null) return null;
            //var books = user.books.Where(b => !b.IsCanceled);
            //return _mapper.Map<List<TicketDTO>>(books);
           return await _context.bookings
                .Where(b => b.UserId == userId && !b.IsCanceled)
                .AsNoTracking()
                .ProjectTo<TicketDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
        public async Task<List<TicketDTO>> GetUserCanceledTicektsAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return null;
           return await _context.bookings
                .Where(b => b.UserId == userId && b.IsCanceled)
                .AsNoTracking().
                ProjectTo<TicketDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
        public async Task<List<TicketDTO>> GetUserHistoryTicketsAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return null;
            var today = DateOnly.FromDateTime(DateTime.Now);
            var nowTime = TimeOnly.FromDateTime(DateTime.Now);
            return await _context.bookings
                .Where(b => b.UserId == userId && (b.trip.Date < today || b.trip.Date == today && b.trip.Time < nowTime))
                .AsNoTracking()
                .ProjectTo<TicketDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<TicketDTO?> GetTicketAsync(int id)
        {
             
            return await _context.bookings
                .Where(b => b.BookingId == id)
                .ProjectTo<TicketDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
          
        }
        public async Task<ApiResponse<string>> BookForGuestAsync(int TripId,PassengerDTO DTO)
        {
            string FirstName = DTO.FirstName;
            string LastName = DTO.LastName;
            string PhoneNumber = DTO.PhoneNumber;
            var seats = DTO.Seats;
            var Trip=await _context.Trips.Include(t=>t.seats).FirstOrDefaultAsync(t=>t.TripId==TripId);
            if (Trip == null||Trip.Avaliblility==false)
            {
                return ApiResponse<string>.Error("حدث خطا");
            }
            if (Trip.seats == null)
            {
                Trip.seats = new List<Seat>();
            }
            var isbooked = DTO.Seats.Any(number => Trip.seats.Any(s => s.Number == number && s.State == SeatState.Booked));
            if (isbooked)
            {
                return ApiResponse<string>.Error("بعض المقاعد التي اخترتها محجوزة بالفعل");
            }
            int count = DTO.Seats.Count;
            double total = Trip.Price * count;
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var booking = new Booking
                {
                    trip = Trip,
                    tripId = Trip.TripId,
                    GuestFirstName=DTO.FirstName,
                    GuestLastName=DTO.LastName,
                    GuestPhoneNumber=DTO.PhoneNumber,
                    IsCanceled = false,
                    seats = new List<Seat>()
                };

                await _context.bookings.AddAsync(booking);
                await _context.SaveChangesAsync();

                var Seats = DTO.Seats.Select(seatNumber => new Seat
                {
                    Number = seatNumber,
                    State = SeatState.Booked,
                    booking = booking,
                    trip = Trip,
                    bookingId = booking.BookingId,
                    TripId = Trip.TripId
                }).ToList();

                booking.seats.AddRange(Seats);
                await _context.SaveChangesAsync();
                var payment = new Payment
                {
                    Method = "Cash",
                    Date = DateTime.Now,
                    amount = total,
                    bookingId = booking.BookingId,
                    IsRefunded = false,
                    booking = booking,
                    PaymentIntentId = "Cash_" + new Random().Next(100000, 999999).ToString(),
                };


                await _context.Payments.AddAsync(payment);
                await _context.SaveChangesAsync();
                booking.payment = payment;
                booking.PaymentId = payment.PaymentId;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return ApiResponse<string>.success("تم الحجز بنجاح");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return ApiResponse<string>.Error("حدث خطا");

            }

        }
      //  public async Task<ApiResponse<string?>>Cancel
    }
    }

