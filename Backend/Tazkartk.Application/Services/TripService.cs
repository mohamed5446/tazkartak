using AutoMapper;
using ClosedXML.Excel;
using Tazkartk.Domain.Models.Enums;
using Tazkartk.Domain.Models;
using Microsoft.AspNetCore.Http;
using Tazkartk.Application.DTO.TripDTOs;
using Tazkartk.Application.DTO;
using Tazkartk.Application.DTO.Response;
using Tazkartk.Application.Interfaces;
using Tazkartk.Application.Repository;
using Tazkartk.Application.DTO.Email;
using Tazkartk.Application.Interfaces.External;
namespace Tazkartk.Application.Services
{
    public class TripService : ITripService
    {
        private readonly IEmailService _EmailService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBackgroundService _backgroundService;
        private readonly IExcelService _excelService;
        private readonly IEmailBodyService _emailBodyService;
        public TripService(IEmailService emailService, IMapper mapper, IUnitOfWork unitOfWork, IBackgroundService backgroundService, IExcelService excelService, IEmailBodyService emailBodyService)
        {
            _EmailService = emailService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _backgroundService = backgroundService;
            _excelService = excelService;
            _emailBodyService = emailBodyService;
        }

        public async Task<IReadOnlyList<TripDtos>> GetTripsAsync()
        {
            return await _unitOfWork.Trips.ProjectToList<TripDtos>();                  
        }
        public async Task<IReadOnlyList<TripDtos>> GetAvailableTripsAsync()
        {
            return await _unitOfWork.Trips.ProjectToList<TripDtos>(t => t.Avaliblility);
        }
        public async Task<IReadOnlyList<TripDtos>?> GetCompanyTripsAsync(int companyId)
        {
            var company = await _unitOfWork.Companies.GetById(c => c.Id == companyId, c => c.Trips);
            if (company == null) return null;
            return _mapper.Map<IReadOnlyList<TripDtos>>(company.Trips.Where(t => t.Avaliblility));
        }
        public async Task<IReadOnlyList<TripDtos>> SearchAsync(string? from, string? to, DateOnly? date)
        {
            return await _unitOfWork.Trips.ProjectToList<TripDtos>(s =>
                      (from == null || s.From == from)
                   && (to == null   || s.To == to)
                   && (date == null || s.Date == date)
                   && s.Avaliblility);           
        }
        public async Task<TripDTO?> GetTripByIdAsync(int id)
        {
            return await _unitOfWork.Trips.GetById<TripDTO>(t => t.TripId == id);
        }
        public async Task<IEnumerable<PassengerDetailsDTO>> GetPassengersAsync(int TripId)
        {
            return  await _unitOfWork.Trips.GetPassengersAsync(TripId);
        }
        public async Task<IReadOnlyList<TicketDTO>?> GetBookingsByTripAsync(int TripId)
        {
           var Trip=await _unitOfWork.Trips.GetById(TripId);
            if (Trip == null) return null;
            var today = DateOnly.FromDateTime(DateTime.Now);
            var nowTime = TimeOnly.FromDateTime(DateTime.Now);
            return  _mapper.Map<IReadOnlyList<TicketDTO>>(Trip.bookings);
        }
        public async Task<ApiResponse<TripDtos>> AddTripAsync(int CompanyId, CreateTripDtos TripDtos)
        {
            var company=await _unitOfWork.Companies.GetById(CompanyId);
            if (company == null)
            {
                return ApiResponse<TripDtos>.Error("الشركة غير موجودة ");
            }
            var DepartureTime = TripDtos.Date.ToDateTime(TripDtos.Time);
            if (DepartureTime <= DateTime.Now)
            {
                return ApiResponse<TripDtos>.Error("حدث خطا");
            }
            if (TripDtos.Price <= 0)
            {
                return ApiResponse<TripDtos>.Error("يجب أن يكون سعر الرحلة أكبر من صفر");
            }
            var Tripmodel = _mapper.Map<Trip>(TripDtos);
            company.Trips.Add(Tripmodel);
            Tripmodel.CompanyId = company.Id;
            await _unitOfWork.Trips.Add(Tripmodel);
            await _unitOfWork.CompleteAsync();

            //var DepartureDateTime = Tripmodel.Date.ToDateTime(Tripmodel.Time);
            //var reminderTime = DepartureDateTime.AddHours(-2);

            scheduleTripJobs(Tripmodel.TripId, Tripmodel.Date,Tripmodel.Time);

            var Data = _mapper.Map<TripDtos>(Tripmodel);
            return ApiResponse<TripDtos>.success("تم اضافة الرحلة بنجاح", Data, StatusCode.Created);
        }
     
        public async Task<ApiResponse<string>> DeleteTripAsync(int id)
        {
            var Tripmodel = await _unitOfWork.Trips.GetById(t => t.TripId == id, t => t.seats);
            if (Tripmodel == null)
            {
                return ApiResponse<string>.Error("الرحلة غير موجودة ");
            }
            if (Tripmodel.seats.Any(s => s.State == SeatState.Booked))
            {
                return ApiResponse<string>.Error("لا يمكن حذف الرحلة لأن هناك حجوزات مرتبطة بها. يُرجى التأكد من عدم وجود حجوزات أولاً.");
            }
           await _backgroundService.DeleteExistingJobs(id);
            _unitOfWork.Trips.Remove(Tripmodel);
           await  _unitOfWork.CompleteAsync();

            return ApiResponse<string>.success("تم حذف الرحلة بنجاح");
        }
        public async Task<ApiResponse<TripDtos>> EditTripAsync(int id, UpdateTripDtos UpdateDtos)
        {
            var Tripmodel =await  _unitOfWork.Trips.GetById(t => t.TripId == id, t => t.company);
            if (Tripmodel == null)
            {
                return ApiResponse<TripDtos>.Error("الرحلة غير موجودة ");
            }

            if (UpdateDtos.Date.HasValue && UpdateDtos.Date.Value != Tripmodel.Date || UpdateDtos.Time.HasValue && UpdateDtos.Time != Tripmodel.Time)
            {
                _backgroundService.DeleteExistingJobs(id);
                scheduleTripJobs(id, UpdateDtos.Date.Value, UpdateDtos.Time.Value);
            }
            Tripmodel.From = UpdateDtos.From ?? Tripmodel.From;
            Tripmodel.To = UpdateDtos.To ?? Tripmodel.To;
            Tripmodel.Class = UpdateDtos.Class ?? Tripmodel.Class;
            Tripmodel.Location = UpdateDtos.Location ?? Tripmodel.Location;
            Tripmodel.Price = UpdateDtos.Price ?? Tripmodel.Price;
            Tripmodel.Date = UpdateDtos.Date ?? Tripmodel.Date;
            Tripmodel.Time = UpdateDtos.Time ?? Tripmodel.Time;
            Tripmodel.Avaliblility = UpdateDtos.Avaliblility ?? Tripmodel.Avaliblility;

            _unitOfWork.Trips.Update(Tripmodel);
            await _unitOfWork.CompleteAsync();
            var Data = _mapper.Map<TripDtos>(Tripmodel);
            return ApiResponse<TripDtos>.success("تم التعديل بنجاح", Data);
        }
        public async Task<bool> SendReminderEmailAsync(int TripId)
        {
            var users = await _unitOfWork.Bookings.ProjectToList<TicketDTO>(b => b.UserId != null);
            foreach (var user in users)
            {
                var emailrequest = new EmailRequest
                {
                    Email = user.userEmail,
                    Subject = "تذكير بالرحلة",
                    Body = _emailBodyService.TripReminderEmailBody(user)// EmailBodyHelper.RemiderEmailBody(user),//(user.Name, user.CompanyName, user.DepartureDate, user.DepartureTime, user.From, user.To, user.SeatsNumbers) // $"{user.FirstName}، باقي على رحلتك  2 ساعة. نتمنى لك رحلة سعيدة 🚌"
                };
                await _EmailService.SendEmail(emailrequest);
            }
            return true;
        }
        public async Task<bool> MarkTripUnavailableAsync(int TripId)
        {
            var Trip = await _unitOfWork.Trips.GetById(TripId);
            if (Trip == null) return false;
            Trip.Avaliblility = false;
            _unitOfWork.Trips.Update(Trip);
            _unitOfWork.CompleteAsync();    
            return true;
        }
        public async Task<bool> send_Email_to_passengersAsync(int TripId, EmailDTO DTO)
        {
            var Trip = await _unitOfWork.Trips.GetTripWithBookingAndUer(TripId);// then include user
            if (Trip == null) return false;
            var users = Trip.bookings.Where(b => b.UserId != null).Select(b => new PassDTO
            {
                Email = b.user.Email,
                FirstName = b.user.FirstName,
                LastName = b.user.LastName,
            });

            foreach (var passenger in users)
            {
                var emailrequest = new EmailRequest
                {
                    Email = passenger.Email,
                    Subject = DTO.Subject,
                    Body = DTO.Body
                };
                await _EmailService.SendEmail(emailrequest);
            }
            return true;
        }

        public async Task<List<CreateTripDtos>>ImportFromExcelAsync(int CompanyId,IFormFile file)
        {
            var trips = new List<CreateTripDtos>();

            var importedTrips = await _excelService.Import<CreateTripDtos>(file, async (sheet, row) =>
            {
                var from = sheet.Cell(row, 1).GetString().Trim();
                var to = sheet.Cell(row, 2).GetString().Trim();
                var tripClass = sheet.Cell(row, 3).GetString().Trim();

                var dateCell = sheet.Cell(row, 4);
                if (!TryGetDateOnly(dateCell, out var date))
                    throw new Exception("تاريخ غير صالح");

                var timeCell = sheet.Cell(row, 5);
                if (!TryGetTimeOnly(timeCell, out var time))
                    throw new Exception("وقت غير صالح");

                var location = sheet.Cell(row, 6).GetString().Trim();
                var priceStr = sheet.Cell(row, 7).GetString().Trim();

                if (!double.TryParse(priceStr, out var price))
                    throw new Exception("سعر غير صالح");

                var dto = new CreateTripDtos
                {
                    From = from,
                    To = to,
                    Class = tripClass,
                    Date = date,
                    Time = time,
                    Location = location,
                    Price = price
                };
                return dto;

            });

            trips.AddRange(importedTrips);
            var Trips=await _unitOfWork.Trips.AddRange(trips,CompanyId);
            foreach(Trip trip in Trips)
            {
                scheduleTripJobs(trip.TripId, trip.Date,trip.Time);
            }
            await _unitOfWork.CompleteAsync();
            return trips;
        }

        private bool TryGetDateOnly(IXLCell cell, out DateOnly date)
        {
            if (cell.DataType == XLDataType.DateTime)
            {
                date = DateOnly.FromDateTime(cell.GetDateTime());
                return true;
            }
            else if (DateTime.TryParse(cell.GetString(), out var dt))
            {
                date = DateOnly.FromDateTime(dt);
                return true;
            }

            date = default;
            return false;
        }
        private bool TryGetTimeOnly(IXLCell cell, out TimeOnly time)
        {
            if (cell.DataType == XLDataType.DateTime)
            {
                time = TimeOnly.FromDateTime(cell.GetDateTime());
                return true;
            }
            else if (DateTime.TryParse(cell.GetString(), out var dt))
            {
                time = TimeOnly.FromDateTime(dt);
                return true;
            }

            time = default;
            return false;
        }
        public void scheduleTripJobs(int TripId, DateOnly Date, TimeOnly Time)
        {
            TimeZoneInfo egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");

            var departureDateTimeLocal = new DateTime(Date.Year, Date.Month, Date.Day, Time.Hour, Time.Minute, 0);
            var departureDateTimeUtc = TimeZoneInfo.ConvertTimeToUtc(departureDateTimeLocal, egyptTimeZone);

            var departureDateTimeOffset = new DateTimeOffset(departureDateTimeUtc);

            var reminderTimeUtc = departureDateTimeUtc.AddHours(-2);
            var reminderTimeOffset = new DateTimeOffset(reminderTimeUtc);

            var jobId = _backgroundService.AddSchedule<TripService>(
                service => service.SendReminderEmailAsync(TripId),
                reminderTimeOffset);

            var job2Id = _backgroundService.AddSchedule<TripService>(
                service => service.MarkTripUnavailableAsync(TripId),
                departureDateTimeOffset);

            var job3Id = _backgroundService.AddSchedule<PaymentService>(
                service => service.TransferFunds(TripId),
                departureDateTimeOffset);

        }
        public async Task deletejobs(int tripId)
        {
           await _backgroundService.DeleteExistingJobs(tripId);
        
        }
        public void deletejob(string jobId)
        {
            _backgroundService.DeleteJob(jobId);
        }

       
    }
      
    }



