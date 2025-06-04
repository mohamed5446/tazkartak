using AutoMapper;
using Tazkartk.Domain.Models;
using Tazkartk.Domain.Models.Enums;
using Tazkartk.Application.Interfaces;
using Tazkartk.Application.DTO;
using Tazkartk.Application.DTO.Response;
using Tazkartk.Application.DTO.UserDTOs;
using Tazkartk.Application.Repository;
using Tazkartk.Application.Interfaces.External;
using Tazkartk.Application.DTO.Payments;

namespace Tazkartk.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IMapper _mapper;
        private readonly IPaymentGateway _paymentGateway;
        private readonly IUnitOfWork _unitOfWork;
        public BookingService(  IMapper mapper, IPaymentGateway paymentGateway,IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _paymentGateway = paymentGateway;
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<TicketDTO>> GetBookingsAsync()
        {
            return await _unitOfWork.Bookings.ProjectToList<TicketDTO>();
        }


        public async Task<IReadOnlyList<TicketDTO>?> GetUserBookingsAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetUserWtihBookingDetails(userId);
            if (user == null) return null;

            return _mapper.Map<List<TicketDTO>>(user.books
                .Where(b => b.trip.Avaliblility));

        }

        public async Task<IReadOnlyList<TicketDTO>> GetUserHistoryTicketsAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetUserWtihBookingDetails(userId);
            if (user == null) return null;
            var today = DateOnly.FromDateTime(DateTime.Now);
            var nowTime = TimeOnly.FromDateTime(DateTime.Now);
            return _mapper.Map<IReadOnlyList<TicketDTO>>(user.books
                .Where(b => b.UserId == userId
                && (b.trip.Date < today
                || b.trip.Date == today
                && b.trip.Time < nowTime)));

        }
        public async Task<TicketDTO?> GetTicketAsync(int id)
        {
            return await _unitOfWork.Bookings.GetById<TicketDTO>(t => t.BookingId == id);
        }

        #region Booking
        public async Task<ApiResponse<string>> BookSeatAsync(BookingDTO DTO)
        {
            var user = await _unitOfWork.Users.GetById(DTO.UserId);
           
            var trip=await _unitOfWork.Trips.GetById(t=>t.TripId==DTO.TripId,t=>t.seats);
          
            if (trip == null || user == null || trip.Avaliblility == false)
            {
                return ApiResponse<string>.Error("حدث خطا");
            }
           
            var UserDetailsDTO = _mapper.Map<UserDetailsDTO>(user);

            var isbooked = DTO.SeatsNumbers.Any(number => trip.seats.Any(s => s.Number == number && s.State == SeatState.Booked));

            if (isbooked)
            {
                return ApiResponse<string>.Error("بعض المقاعد التي اخترتها محجوزة ");
            }
           
            int count = DTO.SeatsNumbers.Count;
            double total = trip.Price * count;
            var BookingDetailDTO = new UserBookingDetails
            {
                firstName=UserDetailsDTO.FirstName, 
                lastName=UserDetailsDTO.LastName,
                email=UserDetailsDTO.Email,
                phoneNumber=UserDetailsDTO.PhoneNumber,
                userId=DTO.UserId,
                tripId=DTO.TripId,
                seatNumbers=DTO.SeatsNumbers
            };
            string Url = await _paymentGateway.CreatePaymentIntentAsync(BookingDetailDTO, total);
            if (!string.IsNullOrEmpty(Url))
            {
                return ApiResponse<string>.success("payment url", Url);
            }
            return ApiResponse<string>.Error("حدث خطا");
        }
        public async Task<bool> ConfirmBookingAsync(BookingDTO DTO, string PaymentIntentId, string PaymentMethod)
        {
           var user = await _unitOfWork.Users.GetUserWithBookingandTrip(DTO.UserId);
          
            var trip = await _unitOfWork.Trips.GetById(t => t.TripId == DTO.TripId, t => t.seats);
           

            if (user == null || trip == null) return false;
            var booking = new Booking
            {
                trip = trip,
                tripId = trip.TripId,
                user = user,
                UserId = user.Id,
                IsCanceled = false,

            };

            var seats = DTO.SeatsNumbers.Select(seatNumber => new Seat
            {
                Number = seatNumber,
                State = SeatState.Booked,
                booking = booking,
                trip = trip,
                bookingId = booking.BookingId,
                TripId = trip.TripId

            }).ToList();


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
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                booking.seats.AddRange(seats);

                booking.payment = payment;
                await _unitOfWork.Bookings.Add(booking);
                await _unitOfWork.Payments.Add(payment);
                await _unitOfWork.commitTransactionAsync();
                return true;
            }
            catch
            {
                await _unitOfWork.RollBackAsdync();
                return false;
            }                 
        }
        public async Task<ApiResponse<string>> BookForGuestAsync(int TripId, PassengerDTO DTO)
        {
            var Trip = await _unitOfWork.Trips.GetById(t => t.TripId == TripId, t => t.seats);
            if (Trip == null || Trip.Avaliblility == false)
            {
                return ApiResponse<string>.Error("حدث خطا");
            }
           
            var isbooked = DTO.Seats.Any(number => Trip.seats.Any(s => s.Number == number && s.State == SeatState.Booked));
            if (isbooked)
            {
                return ApiResponse<string>.Error("بعض المقاعد التي اخترتها محجوزة بالفعل");
            }
            int count = DTO.Seats.Count;
            double total = Trip.Price * count;

            var booking = new Booking
            {
                trip = Trip,
                tripId = Trip.TripId,
                GuestFirstName = DTO.FirstName,
                GuestLastName = DTO.LastName,
                GuestPhoneNumber = DTO.PhoneNumber,
                IsCanceled = false,
            };

            var Seats = DTO.Seats.Select(seatNumber => new Seat
            {
                Number = seatNumber,
                State = SeatState.Booked,
                booking = booking,
                trip = Trip,
                bookingId = booking.BookingId,
                TripId = Trip.TripId
            }).ToList();

            var payment = new Payment
            {
                Method = PaymentMethods.Cash.ToString(),
                Date = DateTime.Now,
                amount = total,
                bookingId = booking.BookingId,
                IsRefunded = false,
                booking = booking,
                PaymentIntentId = PaymentMethods.Cash.ToString() + '_' + new Random().Next(100000, 999999).ToString(),
            };
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                booking.seats.AddRange(Seats);

                booking.payment = payment;
                await _unitOfWork.Bookings.Add(booking);
                await _unitOfWork.Payments.Add(payment);
                await _unitOfWork.commitTransactionAsync();
                return ApiResponse<string>.success("تم الحجز بنجاح");

            }
            catch
            {
                await _unitOfWork.RollBackAsdync();
                return ApiResponse<string>.Error("حدث خطا");
            }
            
        }

        #endregion

        #region Cancel
        public async Task<ApiResponse<bool>> RefundAsync(int bookingId)
        {
            var booking = await _unitOfWork.Bookings.GetById(b => b.BookingId == bookingId, b => b.seats, b => b.trip, b => b.payment);
          //  var booking = await _ticketRepository.GetTicketById(t => t.BookingId == bookingId, t => t.seats, t => t.trip, t => t.payment);
           // var booking = await _context.bookings.Include(b => b.seats).Include(b => b.trip).Include(b => b.payment).FirstOrDefaultAsync(b => b.BookingId == bookingId);
            if (booking == null)
            {
                return ApiResponse<bool>.Error("حدث خطا");
            }
            var DepartureTime = booking.trip.Date.ToDateTime(booking.trip.Time);
            if (DateTime.Now > DepartureTime.AddHours(-2))
            {
                return ApiResponse<bool>.Error("لا يمكن الغاء الرحلة باقي اقل من ساعتان");
            }
            if (booking.IsCanceled == true)
            {
                return ApiResponse<bool>.Error("لقد قمت بالغاء الحجز");
            }
            var count = booking.seats.Count();
            var total = count * booking.trip.Price * 100;
            var paymemnt = await _unitOfWork.Payments.GetById(p => p.bookingId == bookingId);
            // var paymemnt=await _paymentRepository.GetPaymentByBookingId(bookingId);
            //var paymemnt = await _context.Payments.FirstOrDefaultAsync(p => p.bookingId == bookingId);
            if (paymemnt == null || paymemnt.IsRefunded)
            {
                return ApiResponse<bool>.Error("حدث خطا");
            }
            if (paymemnt.Method == "wallet")
            {
                ApiResponse<bool>.Error("الاسترداد عبر المحفظة غير متاح في الوقت الحالي");
            }
           
            var trxId = paymemnt.PaymentIntentId;
            var result = await _paymentGateway.RefundTransactionAsync( trxId, total);
            if (!result) return ApiResponse<bool>.Error("حدث خطا");
            return ApiResponse<bool>.success("تم تحويل المبلغ . يرجى التأكد من حسابك، وإذا واجهت أي مشكلة،برجاء التواصل معنا. ");
        }
        public async Task<bool> CancelAsync(string trxId)
        {
            var payment =await _unitOfWork.Payments.GetById(p=>p.PaymentIntentId==trxId);
          //  var payment=await _paymentRepository.GetPaymentByIntentId(trxId);
            //var payment = await _context.Payments.FirstOrDefaultAsync(p => p.PaymentIntentId == trxId);
            if (payment == null) return false;
            var paymentId = payment.PaymentId;
            var booking = await _unitOfWork.Bookings.GetById(b => b.PaymentId == paymentId, b => b.seats, b => b.trip);
           // var booking = await _ticketRepository.getTicketWithSeatsAndTheirTripsByPaymentId(paymentId);
            //var booking = await _context.bookings.Include(b => b.seats).Include(b => b.trip).FirstOrDefaultAsync(b => b.PaymentId == paymentId);
            if (booking == null) return false;
            var count = booking.seats.Count;
            var total = count * booking.trip.Price * 100;

            foreach (var seat in booking.seats)
            {
                seat.State = SeatState.Available;
            }
            payment.IsRefunded = true;
            booking.IsCanceled = true;
            _unitOfWork.Bookings.Update(booking);
           await  _unitOfWork.CompleteAsync();
          //  _ticketRepository.UpdateTicket(booking);
            //await _context.SaveChangesAsync();
            return true;
        }
        #endregion

        public async Task<ApiResponse<string>> DeleteBookingAsync(int Id)
        {
            var booking = await _unitOfWork.Bookings.GetById(b => b.BookingId == Id, b => b.seats,b=>b.payment);
            _unitOfWork.Bookings.DeleteTicket(booking);
            await _unitOfWork.CompleteAsync();  
            return ApiResponse<string>.success("تم حذف الحجز");
        }



      
       
    }

}

