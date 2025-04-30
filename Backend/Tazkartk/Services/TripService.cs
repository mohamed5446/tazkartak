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

        public async Task<IEnumerable<TripDtos>> GetTripsAsync()
        {
            return await _context.Trips.ProjectTo<TripDtos>(_mapper.ConfigurationProvider).AsNoTracking().ToListAsync();
            //return  await _mapper.ProjectTo<TripDtos>(_context.Trips.AsNoTracking()).ToListAsync();
        }
        public async Task<IEnumerable<TripDtos>> GetAvailableTripsAsync()
        {
            return await _context.Trips.Where(t => t.Avaliblility).ProjectTo<TripDtos>(_mapper.ConfigurationProvider).AsNoTracking().ToListAsync();
            //return await _mapper.ProjectTo<TripDtos>(_context.Trips.Where(t => t.Avaliblility == true).AsNoTracking()).ToListAsync();
        }
        public async Task<IEnumerable<TripDtos>?> GetCompanyTripsAsync(int companyId)
        {

            var company = await _context.Companies
                .Include(c => c.Trips)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == companyId);
            if (company == null) return null;
           return  _mapper.Map<IEnumerable<TripDtos>>(company.Trips.Where(t=>t.Avaliblility));
        }
        public async Task<IEnumerable<TripDtos>> SearchAsync(string? from, string? to, DateOnly? date)
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
            return await _context.Trips
                 .Where(t => t.TripId == TripId)
                 .SelectMany(t => t.bookings)
                 .Where(b => !b.IsCanceled)
                 .ProjectTo<PassengerDetailsDTO>(_mapper.ConfigurationProvider)
                 .AsNoTracking()
                 .ToListAsync();
        }
        public async Task<List<TicketDTO>?> GetBookingsByTripAsync(int TripId)
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
            var job3Id = BackgroundJob.Schedule<TripService>(service => service.transfer(Tripmodel.TripId), DepartureDateTime);

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

            if (!string.IsNullOrEmpty(UpdateDtos.From)) Tripmodel.From = UpdateDtos.From;
            if (!string.IsNullOrEmpty(UpdateDtos.To)) Tripmodel.To = UpdateDtos.To;
            if (!string.IsNullOrEmpty(UpdateDtos.Class)) Tripmodel.Class = UpdateDtos.Class;
            if (!string.IsNullOrEmpty(UpdateDtos.Location)) Tripmodel.Location = UpdateDtos.Location;
            if (UpdateDtos.Price.HasValue) Tripmodel.Price = UpdateDtos.Price.Value;
            if (UpdateDtos.Date.HasValue) Tripmodel.Date = UpdateDtos.Date.Value;
            if (UpdateDtos.Time.HasValue) Tripmodel.Time = UpdateDtos.Time.Value;
           // if (UpdateDtos.ArriveTime.HasValue) Tripmodel.ArriveTime = UpdateDtos.ArriveTime.Value;
            if (UpdateDtos.Avaliblility.HasValue) Tripmodel.Avaliblility = UpdateDtos.Avaliblility.Value;
            // if (UpdateDtos.TripCode.HasValue) Tripmodel.TripCode = UpdateDtos.TripCode.Value;

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
                var t = (int)j.Value.Job.Args[0];
                if (t == tripId)
                {
                    BackgroundJob.Delete(j.Key);
                }
            }

        }
        public async Task<bool> transfer(int TripId)
        {
            var trip = await _context.Trips.Include(t => t.seats).Include(t => t.company).FirstOrDefaultAsync(t => t.TripId == TripId);
            if (trip == null) throw new Exception("error happened");
            var count = trip.seats.Where(s => s.State == SeatState.Booked).Count();
            var total = count * trip.Price;
            var company = trip.company;
            company.Balance+=total;
            _context.Companies.Update(company);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}


