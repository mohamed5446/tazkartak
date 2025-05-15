using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Tazkartk.Data;
using Tazkartk.DTO.Response;
using Tazkartk.DTO.TripDTOs;
using Tazkartk.Interfaces;
using Tazkartk.Models.Enums;
using Tazkartk.DTO.UserDTOs;
using Tazkartk.Email;
using Hangfire;
using Tazkartk.DTO;
using Tazkartk.Models;
using Tazkartk.Helpers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ClosedXML.Excel;
using System;

namespace Tazkartk.Services
{
    public class TripService : ITripService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _EmailService;
        private readonly IMapper _mapper;
        public TripService(ApplicationDbContext context, IEmailService emailService, IMapper mapper)
        {
            _context = context;
            _EmailService = emailService;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<TripDtos>> GetTripsAsync()
        {
            return await _context.Trips.ProjectTo<TripDtos>(_mapper.ConfigurationProvider).AsNoTracking().ToListAsync();
            //return  await _mapper.ProjectTo<TripDtos>(_context.Trips.AsNoTracking()).ToListAsync();
        }
        public async Task<IReadOnlyList<TripDtos>> GetAvailableTripsAsync()
        {
            return await _context.Trips.Where(t => t.Avaliblility).ProjectTo<TripDtos>(_mapper.ConfigurationProvider).AsNoTracking().ToListAsync();
            //return await _mapper.ProjectTo<TripDtos>(_context.Trips.Where(t => t.Avaliblility == true).AsNoTracking()).ToListAsync();
        }
        public async Task<IReadOnlyList<TripDtos>?> GetCompanyTripsAsync(int companyId)
        {

            var company = await _context.Companies
                .Include(c => c.Trips)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == companyId);
            if (company == null) return null;
           return  _mapper.Map<IReadOnlyList<TripDtos>>(company.Trips.Where(t=>t.Avaliblility));
        }
        public async Task<IReadOnlyList<TripDtos>> SearchAsync(string? from, string? to, DateOnly? date)
        {
         return  await _context.Trips
                .Where(s => 
                   (from == null || s.From == from)
                && (to == null || s.To == to) 
                && (date == null || s.Date == date)
                && s.Avaliblility)               
                .ProjectTo<TripDtos>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<TripDTO?> GetTripByIdAsync(int id)
        {
        return  await _context.Trips
               .Where(t => t.TripId == id)
               .ProjectTo<TripDTO>(_mapper.ConfigurationProvider)
               .AsNoTracking()
               .FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<PassengerDetailsDTO>> GetPassengersAsync(int TripId)
        {
            var userPassengers = await _context.bookings
        .Where(b => b.tripId == TripId && !b.IsCanceled && b.user != null)
        .GroupBy(b => b.user)
        .Select(g => new PassengerDetailsDTO
        {
            FirstName= g.Key.FirstName,
            LastName = g.Key.LastName,
            Email = g.Key.Email,
            PhoneNumber = g.Key.PhoneNumber,
            Seats = g.SelectMany(b => b.seats.Select(s=>s.Number)).Distinct().ToList()
        })
        .ToListAsync();

            var guestPassengers = await _context.bookings
                .Where(b => b.tripId == TripId && !b.IsCanceled && b.user == null)
                .GroupBy(b => new { b.GuestFirstName, b.GuestLastName,b.GuestPhoneNumber})
                .Select(g => new PassengerDetailsDTO
                {
                   FirstName=g.Key.GuestFirstName,
                   LastName=g.Key.GuestLastName,
                    PhoneNumber = g.Key.GuestPhoneNumber,
                    Email = null,
                    Seats = g.SelectMany(b => b.seats.Select(s=>s.Number)).Distinct().ToList()
                })
                .ToListAsync();

            return userPassengers.Concat(guestPassengers);


            //return await _context.bookings
            //    .Where(b=>!b.IsCanceled&&b.tripId == TripId)
            //    .GroupBy(b=>b.user)
            //    .ProjectTo<PassengerDetailsDTO>(_mapper.ConfigurationProvider)
            //    .AsNoTracking()
            //    .ToListAsync();
            //return await _context.Trips
            //     .Where(t => t.TripId == TripId)
            //     .SelectMany(t => t.bookings)
            //     .Where(b => !b.IsCanceled)
            //     .ProjectTo<PassengerDetailsDTO>(_mapper.ConfigurationProvider)
            //     .AsNoTracking()
            //     .ToListAsync();
        }
        public async Task<IReadOnlyList<TicketDTO>?> GetBookingsByTripAsync(int TripId)
        {
            var Trip = await _context.Trips.FindAsync(TripId);
            if (Trip == null) return null;
            var today = DateOnly.FromDateTime(DateTime.Now);
            var nowTime = TimeOnly.FromDateTime(DateTime.Now);
            return  await _context.bookings
                 .Where(b => b.tripId == TripId )
                 .ProjectTo<TicketDTO>(_mapper.ConfigurationProvider)
                 .AsNoTracking()
                 .ToListAsync();
        }
        public async Task<ApiResponse<TripDtos>> AddTripAsync(int CompanyId, CreateTripDtos TripDtos)
        {
            var company = await _context.Companies
                .Include(c => c.Trips)
                .FirstOrDefaultAsync(c => c.Id == CompanyId);

            if (company == null)
            {
                return ApiResponse<TripDtos>.Error("الشركة غير موجودة ");
            }
            var DepartureTime = TripDtos.Date.ToDateTime(TripDtos.Time);
           if(DepartureTime<=DateTime.Now)
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
            await _context.Trips.AddAsync(Tripmodel);
            await _context.SaveChangesAsync();

            var DepartureDateTime = Tripmodel.Date.ToDateTime(Tripmodel.Time);
            var reminderTime = DepartureDateTime.AddHours(-2);
            var jobId = BackgroundJob.Schedule<TripService>(service => service.SendReminderEmailAsync(Tripmodel.TripId), reminderTime);
            var job2Id = BackgroundJob.Schedule<TripService>(service => service.MarkTripUnavailableAsync(Tripmodel.TripId), DepartureDateTime);
            var job3Id = BackgroundJob.Schedule<TripService>(service => service.TransferFunds(Tripmodel.TripId), DepartureDateTime);

            var Data = _mapper.Map<TripDtos>(Tripmodel);
            return ApiResponse<TripDtos>.success("تم اضافة الرحلة بنجاح", Data, StatusCode.Created);
        }
        public async Task<ApiResponse<string>> DeleteTripAsync(int id)
        { 
            var Tripmodel = await _context.Trips.Include(t=>t.seats).FirstOrDefaultAsync(s => s.TripId == id);    
            if (Tripmodel == null)
             {
              return ApiResponse<string>.Error("الرحلة غير موجودة ");
             }       
             if (Tripmodel.seats.Any(s => s.State == SeatState.Booked))   
             {   
                return ApiResponse<string>.Error("لا يمكن حذف الرحلة لأن هناك حجوزات مرتبطة بها. يُرجى التأكد من عدم وجود حجوزات أولاً.");   
             }   
                    DeleteExistingJobs(id);
                _context.Trips.Remove(Tripmodel);
                await _context.SaveChangesAsync();
                return ApiResponse<string>.success("تم حذف الرحلة بنجاح");
        }
        public async Task<ApiResponse<TripDtos>> EditTripAsync(int id, UpdateTripDtos UpdateDtos)
        {

            var Tripmodel = await _context.Trips.Include(t => t.company).FirstOrDefaultAsync(s => s.TripId == id);
            if (Tripmodel == null)
            {
                return ApiResponse<TripDtos>.Error("الرحلة غير موجودة ");
            }

            if ((UpdateDtos.Date.HasValue&&UpdateDtos.Date.Value!=Tripmodel.Date)||(UpdateDtos.Time.HasValue&&UpdateDtos.Time!=Tripmodel.Time))
            {
                DeleteExistingJobs(id);
                var DepartureDateTime = Tripmodel.Date.ToDateTime(Tripmodel.Time);
                var reminderTime = DepartureDateTime.AddHours(-2);
                var jobId = BackgroundJob.Schedule<TripService>(service => service.SendReminderEmailAsync(Tripmodel.TripId), reminderTime);
                var job2Id = BackgroundJob.Schedule<TripService>(service => service.MarkTripUnavailableAsync(Tripmodel.TripId), DepartureDateTime);
            }
             Tripmodel.From = UpdateDtos.From ?? Tripmodel.From;
            Tripmodel.To = UpdateDtos.To?? Tripmodel.To;
             Tripmodel.Class = UpdateDtos.Class?? Tripmodel.Class;
            Tripmodel.Location = UpdateDtos.Location ?? Tripmodel.Location;
            Tripmodel.Price = UpdateDtos.Price ?? Tripmodel.Price;
           Tripmodel.Date = UpdateDtos.Date ?? Tripmodel.Date;
            Tripmodel.Time = UpdateDtos.Time ?? Tripmodel.Time;
            Tripmodel.Avaliblility = UpdateDtos.Avaliblility ?? Tripmodel.Avaliblility;


            // if (UpdateDtos.TripCode.HasValue) Tripmodel.TripCode = UpdateDtos.TripCode.Value;
            // if (UpdateDtos.ArriveTime.HasValue) Tripmodel.ArriveTime = UpdateDtos.ArriveTime.Value;


            await _context.SaveChangesAsync();
           var Data=_mapper.Map<TripDtos>(Tripmodel);
            return ApiResponse<TripDtos>.success("تم التعديل بنجاح", Data);
        }
        public async Task<bool> SendReminderEmailAsync(int TripId)
        {
            var users = await _context.bookings
                .Where(b => b.UserId != null)
                .ProjectTo<TicketDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
            foreach (var user in users)
            {
                var emailrequest = new EmailRequest
                {
                    Email = user.userEmail,
                    Subject = "تذكير بالرحلة",
                    Body =EmailBodyHelper.RemiderEmailBody(user.Name,user.CompanyName,user.DepartureDate,user.DepartureTime,user.From,user.To,user.SeatsNumbers) // $"{user.FirstName}، باقي على رحلتك  2 ساعة. نتمنى لك رحلة سعيدة 🚌"
                };
                await _EmailService.SendEmail(emailrequest);
            }
            return true;
        }
        public async Task<bool> MarkTripUnavailableAsync(int TripId)
        {
            var trip = await _context.Trips.FindAsync(TripId);
            if (trip == null) return false;
            trip.Avaliblility = false;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> send_Email_to_passengersAsync(int TripId, EmailDTO DTO)
        {
            var Trip = await _context.Trips.Include(t => t.bookings).ThenInclude(b => b.user).FirstOrDefaultAsync(t => t.TripId == TripId);
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
        public void DeleteExistingJobs(int tripId)
        {
            var monitor = JobStorage.Current.GetMonitoringApi();

            var jobsScheduled = monitor.ScheduledJobs(0, int.MaxValue);
            //.Where(x => x.Value.Job.Method.Name == "SendReminderEmail"||x.Value.Job.Method.Name== "MarkTripUnavailable");
            foreach (var j in jobsScheduled)
            {
                var job = j.Value.Job;
                if (job != null)
                {
                    var args = j.Value.Job.Args;
                    if (args != null)
                    {
                        if (args[0] is int t && t == tripId)
                        {
                            BackgroundJob.Delete(j.Key);
                        }
                    }
                }
            }
            //var t = (int)j.Value.Job.Args[0];
            //if (t == tripId)
            //{
        }
                    
           
        public async Task<bool> TransferFunds(int TripId)
        {
            var trip = await _context.Trips.Include(t => t.seats).Include(t => t.company).FirstOrDefaultAsync(t => t.TripId == TripId);
            if (trip == null) throw new Exception("error happened");
            var count = trip.seats.Where(s => s.State == SeatState.Booked).Count();
            var total = count * trip.Price;
            var company = trip.company;
            company.Balance+=total;
            _context.Companies.Update(company);
            await _context.SaveChangesAsync();
            return  true;
        }
        public async Task<IReadOnlyList<TripDtos>>ImportFromExcelAsync(int CompanyId,IFormFile file)
        
            {
                var trips = new List<TripDtos>();

                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                using var workbook = new XLWorkbook(stream);
            //  var worksheet = workbook.Worksheets.First();
            foreach (var worksheet in workbook.Worksheets)
            {
                for (int row = 2; row <= worksheet.LastRowUsed().RowNumber(); row++)
                {
                    try
                    {
                        var from = worksheet.Cell(row, 1).GetString().Trim();
                        var to = worksheet.Cell(row, 2).GetString().Trim();
                        var tripClass = worksheet.Cell(row, 3).GetString().Trim();
                        DateOnly date;
                        var dateCell = worksheet.Cell(row, 4);
                        if (dateCell.DataType == XLDataType.DateTime)
                        {
                            date = DateOnly.FromDateTime(dateCell.GetDateTime());
                        }
                        else if (DateTime.TryParse(dateCell.GetString(), out var dtValue))
                        {
                            date = DateOnly.FromDateTime(dtValue);
                        }
                        else
                        {
                            throw new Exception($"{row}: التاريخ غير صالح.");
                        }

                        TimeOnly time;
                        var timeCell = worksheet.Cell(row, 5);
                        if (timeCell.DataType == XLDataType.DateTime)
                        {
                            time = TimeOnly.FromDateTime(timeCell.GetDateTime());
                        }
                        else if (DateTime.TryParse(timeCell.GetString(), out var timeValue))
                        {
                            time = TimeOnly.FromDateTime(timeValue);
                        }
                        else
                        {
                            throw new Exception($" {row}: الوقت غير صالح.");
                        }
                        var location = worksheet.Cell(row, 6).GetString().Trim();
                        var priceStr = worksheet.Cell(row, 7).GetString().Trim();

                        if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to) ||
                            string.IsNullOrWhiteSpace(tripClass) || string.IsNullOrWhiteSpace(location) ||
                            string.IsNullOrWhiteSpace(priceStr))
                        {
                            throw new Exception($" {row}: هناك حقول مطلوبة مفقودة.");
                        }

                        if (!double.TryParse(priceStr, out double price))
                        {
                            throw new Exception($" {row}: السعر غير صالح.");
                        }

                        var createTrip = new CreateTripDtos
                        {
                            From = from,
                            To = to,
                            Class = tripClass,
                            Date = date,
                            Time = time,
                            Location = location,
                            Price = price
                        };

                        var result = await AddTripAsync(CompanyId, createTrip);

                        if (result.Success)
                            trips.Add(result.Data);
                        else
                            throw new Exception($" {row}: {result.message}");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($" {row} {ex.Message}");
                    }
                }
            }
            return trips;

        }
    }
}


