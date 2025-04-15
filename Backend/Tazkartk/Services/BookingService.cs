using Microsoft.EntityFrameworkCore;
using Tazkartk.DTO;
using Tazkartk.DTO.UserDTOs;
using Tazkartk.Interfaces;
using Tazkartk.Models.Enums;
using Tazkartk.Models;
using Tazkartk.Data;
using Tazkartk.DTO.Response;
using System.Globalization;

namespace Tazkartk.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPaymobService _paymobService;
        private const char RightToLeftCharacter = (char)0x200F;

        public BookingService(ApplicationDbContext context, IPaymobService paymobService)
        {
            _context = context;
            _paymobService = paymobService;
        }
        #region Booking
        public async Task<ApiResponse<string?>> BookSeat(BookingDTO DTO)
        {
            var user = await _context.Users.FindAsync(DTO.UserId);
            var trip = await _context.Trips.Include(t => t.seats).FirstOrDefaultAsync(t => t.TripId == DTO.TripId);
            if (trip == null || user == null||trip.Avaliblility==false)
            {
                return new ApiResponse<string?>
                {
                    Success = false,
                    StatusCode = StatusCode.BadRequest,
                    message = "حدث خطا"
                };
            }
           
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
            var isbooked = DTO.SeatsNumbers.Any(number => trip.seats.Any(s => s.Number == number && s.State == SeatState.Booked));

            if (isbooked)
            {
                return new ApiResponse<string?>
                {
                    Success = false,
                    StatusCode = StatusCode.BadRequest,
                    message = "بعض المقاعد التي اخترتها محجوزة بالفعل"
                };
            }
            int count = DTO.SeatsNumbers.Count;
            double total = trip.Price * count;
            var Url = await _paymobService.CreatePaymentIntent(DTO, total, UserDetailsDTO);
            if (!string.IsNullOrEmpty(Url))
            {
                return new ApiResponse<string?>
                {
                    Success = true,
                    StatusCode = StatusCode.Ok,
                    message = "payment url",
                    Data = Url
                };
            }
            return new ApiResponse<string?>
            {
                Success = false,
                StatusCode = StatusCode.BadRequest,
                message = "حدث  خطا"
            };
        }

        public async Task<bool> ConfirmBooking(BookingDTO DTO, string PaymentIntentId, string PaymentMethod)
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
        public async Task<ApiResponse<bool>> Refund(int bookingId)
        {
           
            var booking = await _context.bookings.Include(b => b.seats).Include(b => b.trip).Include(b => b.payment).FirstOrDefaultAsync(b => b.BookingId == bookingId);
            if (booking == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    StatusCode = StatusCode.BadRequest,
                    message = "Booking not found",
                };
            }

            var DepartureTime = booking.trip.Date.ToDateTime(booking.trip.Time);
            if(DateTime.Now>DepartureTime.AddHours(-2))
            {
                return new ApiResponse<bool>
                {
                    Success= false,
                    StatusCode= StatusCode.BadRequest,
                    message="لا يمكن الغاء الرحلة باقي اقل من ساعتان"
                };
            }
            if (booking.IsCanceled==true)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    StatusCode = StatusCode.BadRequest,
                    message = "لقد قمت بالغاء الحجز"
                };
            }
            var count = booking.seats.Count();
            var total = (count * booking.trip.Price) * 100;
            var paymemnt = await _context.Payments.FirstOrDefaultAsync(p => p.bookingId == bookingId);
            if(paymemnt.Method=="wallet")
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    StatusCode = StatusCode.BadRequest,
                    message = "الاسترداد عبر المحفظة غير متاح في الوقت الحالي"
                };
            }
            if (paymemnt == null || paymemnt.IsRefunded)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    StatusCode = StatusCode.BadRequest,
                    message = "حدث خطا"
                };
            }
            var trxId = paymemnt.PaymentIntentId;
            try
            {
                var success = await _paymobService.RefundTransaction(bookingId, trxId, total);

                return new ApiResponse<bool>
                {
                    Success = true,
                    StatusCode = StatusCode.Ok,
                    message = "تم تحويل المبلغ . يرجى التأكد من حسابك، وإذا واجهت أي مشكلة،برجاء التواصل معنا. "
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>
                {
                    Success=false,
                    StatusCode=StatusCode.BadRequest,
                    message = ex.Message,
                };
            }
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





        public async Task<ApiResponse<string>> DeleteBooking(int Id)
        {
            try
            {
                var booking = await _context.bookings.Include(b => b.seats).FirstOrDefaultAsync(b => b.BookingId == Id);
                var payment = await _context.Payments.FindAsync(booking.PaymentId);
                _context.Seats.RemoveRange(booking.seats);
                _context.Payments.Remove(payment);
                _context.bookings.Remove(booking);

                await _context.SaveChangesAsync();
                return new ApiResponse<string>
                {
                    Success = true,
                    StatusCode = StatusCode.Ok,
                    message = "تم حذف الحجز"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>()
                {
                    Success = false,
                    StatusCode = StatusCode.BadRequest,
                    message = "حدث خطا"
                };
            }
        }



        public async Task<List<TicketDTO>?> GetBookings()
        {
            var arabicCulture = new CultureInfo("ar-SA");
            arabicCulture.DateTimeFormat.Calendar = new GregorianCalendar();
            arabicCulture.DateTimeFormat.AMDesignator = "صباحا";
            arabicCulture.DateTimeFormat.PMDesignator = "مساء";
            return await _context.bookings
               .AsNoTracking()
               .Select(b => new TicketDTO
               {
                   UserId = b.user.Id,
                   userEmail = b.user.Email,
                   CompanyName = b.trip.company.Name,
                   From = b.trip.From,
                   To = b.trip.To,
                   DepartureDate = b.trip.Date.ToString("yyyy-MM-dd", arabicCulture),
                   DepartureTime = RightToLeftCharacter + b.trip.Time.ToString("hh:mm tt", arabicCulture),
                   DepartureDay = b.trip.Date.ToString("dddd", arabicCulture),
                   //Date = b.trip.Date.ToString("dddd yyyy-MM-dd", arabicCulture),
                   //Time = b.trip.Time.ToString("HH:mm tt",arabicCulture),
                   BookingId = b.BookingId,
                   Name = b.user.FirstName!=null?b.user.FirstName+" "+b.user.LastName:b.GuestFirstName+" "+b.GuestLastName,
                   IsCanceled = b.IsCanceled,
                  SeatsNumbers = b.seats.Select(s => s.Number).ToList(),
               }).ToListAsync();
        }



        public async Task<List<TicketDTO>?> GetUserBookings(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return null;
            var arabicCulture = new CultureInfo("ar-SA");
            arabicCulture.DateTimeFormat.Calendar = new GregorianCalendar();
            arabicCulture.DateTimeFormat.AMDesignator = "صباحا";
            arabicCulture.DateTimeFormat.PMDesignator = "مساء";
            return await _context.bookings.Where(b => b.UserId == userId&&!b.IsCanceled).AsNoTracking()
               .Select(b => new TicketDTO
               {
                   UserId = b.user.Id,
                   userEmail = b.user.Email,
                   CompanyName = b.trip.company.Name,
                   From = b.trip.From,
                   To = b.trip.To,
                   //Date = b.trip.Date.ToString("dddd yyyy-MM-dd", arabicCulture),
                   //Time = b.trip.Time.ToString("HH:mm tt",arabicCulture),
                   DepartureDate = b.trip.Date.ToString("yyyy-MM-dd", arabicCulture),
                   DepartureTime = RightToLeftCharacter + b.trip.Time.ToString("hh:mm tt", arabicCulture),
                   DepartureDay = b.trip.Date.ToString("dddd", arabicCulture),
                   //ArrivalDate = TripModel.ArriveTime.ToString("yyyy-MM-dd", arabicCulture),
                   //ArrivalTime = RightToLeftCharacter + TripModel.ArriveTime.ToString("hh:mm tt", arabicCulture),
                   //ArrivalDay = TripModel.ArriveTime.ToString("dddd", arabicCulture),
                   BookingId = b.BookingId,
                   Name = b.user.FirstName,
                   IsCanceled = b.IsCanceled,
                  SeatsNumbers = b.seats.Select(s => s.Number).ToList(),
               }).ToListAsync();
        }
        public async Task<List<TicketDTO>?> GetUserCanceledTicekts(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return null;
            var arabicCulture = new CultureInfo("ar-SA");
            arabicCulture.DateTimeFormat.Calendar = new GregorianCalendar();
            arabicCulture.DateTimeFormat.AMDesignator = "صباحا";
            arabicCulture.DateTimeFormat.PMDesignator = "مساء";
            return await _context.bookings.Where(b => b.UserId == userId && b.IsCanceled).AsNoTracking()
               .Select(b => new TicketDTO
               {
                   UserId = b.user.Id,
                   userEmail = b.user.Email,
                   CompanyName = b.trip.company.Name,
                   From = b.trip.From,
                   To = b.trip.To,
                   DepartureDate = b.trip.Date.ToString("yyyy-MM-dd", arabicCulture),
                   DepartureTime = RightToLeftCharacter + b.trip.Time.ToString("hh:mm tt", arabicCulture),
                   DepartureDay = b.trip.Date.ToString("dddd", arabicCulture),
                   BookingId = b.BookingId,
                   Name = b.user.FirstName,
                   IsCanceled = b.IsCanceled,
                   SeatsNumbers = b.seats.Select(s => s.Number).ToList(),
               }).ToListAsync();
        }
        public async Task<List<TicketDTO>?> GetUserHistoryTickets(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return null;
            var arabicCulture = new CultureInfo("ar-SA");
            arabicCulture.DateTimeFormat.Calendar = new GregorianCalendar();
            arabicCulture.DateTimeFormat.AMDesignator = "صباحا";
            arabicCulture.DateTimeFormat.PMDesignator = "مساء";

            var today = DateOnly.FromDateTime(DateTime.Now);
            var nowTime = TimeOnly.FromDateTime(DateTime.Now);

            return await _context.bookings.Where(b => b.UserId == userId && (b.trip.Date<today||b.trip.Date==today&&b.trip.Time<nowTime))
                .AsNoTracking()
               .Select(b => new TicketDTO
               {
                   UserId = b.user.Id,
                   userEmail = b.user.Email,
                   CompanyName = b.trip.company.Name,
                   From = b.trip.From,
                   To = b.trip.To,
                   DepartureDate = b.trip.Date.ToString("yyyy-MM-dd", arabicCulture),
                   DepartureTime = RightToLeftCharacter + b.trip.Time.ToString("hh:mm tt", arabicCulture),
                   DepartureDay = b.trip.Date.ToString("dddd", arabicCulture),
                   BookingId = b.BookingId,
                   Name = b.user.FirstName,
                   IsCanceled = b.IsCanceled,
                   SeatsNumbers = b.seats.Select(s => s.Number).ToList(),
               }).ToListAsync();
        }

        public async Task<TicketDTO?> GetTicket(int id)
        {
            var arabicCulture = new CultureInfo("ar-SA");
            arabicCulture.DateTimeFormat.Calendar = new GregorianCalendar();
            arabicCulture.DateTimeFormat.AMDesignator = "صباحا";
            arabicCulture.DateTimeFormat.PMDesignator = "مساء";
            //var ticket = await _context.bookings.Include(b => b.user).Include(b => b.seats).Include(b => b.trip).ThenInclude(t => t.company).FirstOrDefaultAsync(b => b.BookingId == id);
            //return new TicketDTO
            //{
            //    UserId = ticket.user.Id,
            //    userEmail = ticket.user.Email,
            //    CompanyName = ticket.trip.company.Name,
            //    From = ticket.trip.From,
            //    To = ticket.trip.To,
            //    DepartureDate = ticket.trip.Date.ToString("yyyy-MM-dd", arabicCulture),
            //    DepartureTime = RightToLeftCharacter + ticket.trip.Time.ToString("hh:mm tt", arabicCulture),
            //    DepartureDay = ticket.trip.Date.ToString("dddd", arabicCulture),
            //    BookingId = ticket.BookingId,
            //    Name = ticket.user.FirstName,
            //    IsCanceled = ticket.IsCanceled,
            //    SeatsNumbers = ticket.seats.Select(s => s.Number).ToList(),
            //};

            return await _context.bookings.Where(b => b.BookingId == id).Select(b => new TicketDTO
            {
                UserId = b.user.Id,
                userEmail = b.user.Email,
                CompanyName = b.trip.company.Name,
                From = b.trip.From,
                To = b.trip.To,
                DepartureDate = b.trip.Date.ToString("yyyy-MM-dd", arabicCulture),
                DepartureTime = RightToLeftCharacter + b.trip.Time.ToString("hh:mm tt", arabicCulture),
                DepartureDay = b.trip.Date.ToString("dddd", arabicCulture),
                BookingId = b.BookingId,
                Name = b.user.FirstName,
                IsCanceled = b.IsCanceled,
                SeatsNumbers = b.seats.Select(s => s.Number).ToList(),
            }).FirstOrDefaultAsync();
        }
        public async Task<ApiResponse<string?>> BookForGuest(int TripId,PassengerDTO DTO)
        {
            string FirstName = DTO.FirstName;
            string LastName = DTO.LastName;
            string PhoneNumber = DTO.PhoneNumber;
            var seats = DTO.Seats;
            var Trip=await _context.Trips.Include(t=>t.seats).FirstOrDefaultAsync(t=>t.TripId==TripId);
            if (Trip == null||Trip.Avaliblility==false)
            {
                return new ApiResponse<string?>
                {
                    Success=false,
                    StatusCode=StatusCode.BadRequest,
                    message="حدث خطا"
                };
            }
            if (Trip.seats == null)
            {
                Trip.seats = new List<Seat>();
            }
            var isbooked = DTO.Seats.Any(number => Trip.seats.Any(s => s.Number == number && s.State == SeatState.Booked));
            if (isbooked)
            {
                return new ApiResponse<string?>
                {
                    Success = false,
                    StatusCode = StatusCode.BadRequest,
                    message = "بعض المقاعد التي اخترتها محجوزة بالفعل"
                };
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

                return new ApiResponse<string?>
                {
                    Success = true,
                    StatusCode = StatusCode.Ok,
                    message = "تم الحجز بنجاح"
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                 return new ApiResponse<string?>
            {
                Success = false,
                StatusCode = StatusCode.BadRequest,
                message = "حدث  خطا"
            };
            }

        }
      //  public async Task<ApiResponse<string?>>Cancel
    }
    }

