using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Tazkartk.Data;
using Tazkartk.DTO.Response;
using Tazkartk.DTO.TripDTOs;
using Tazkartk.Interfaces;
using Tazkartk.Mappers;
using Tazkartk.Models.Enums;
using Tazkartk.DTO.UserDTOs;
using Tazkartk.Email;
using Hangfire;

namespace Tazkartk.Services
{
    public class TripService : ITripService
    {
        private readonly ApplicationDbContext _context;
        private const char RightToLeftCharacter = (char)0x200F;
        private readonly IEmailService _EmailService;
        public TripService(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _EmailService = emailService;
        }
        public async Task<IEnumerable<TripDtos>> GetTrips()
        {
            var trips = await _context.Trips.Include(t => t.company).AsNoTracking().Select(trip => trip.ToTripDto()).ToListAsync();
            if (trips == null) return null;
            return trips;
        }
        public async Task<TripDTO?> GetTripById(int id)
        {
            var arabicCulture = new CultureInfo("ar-SA");
            arabicCulture.DateTimeFormat.Calendar = new GregorianCalendar();
            var trip = await _context.Trips.Where(t => t.TripId == id).Select(TripModel => new TripDTO
            {
                TripId = TripModel.TripId,
                From = TripModel.From,
                To = TripModel.To,
                Class = TripModel.Class,

                DepartureDate = TripModel.Date.ToString("yyyy-MM-dd", arabicCulture),
                DepartureTime = RightToLeftCharacter + TripModel.Time.ToString("hh:mm tt", arabicCulture),
                DepartureDay = TripModel.Date.ToString("dddd", arabicCulture),
                ArrivalDate = TripModel.ArriveTime.ToString("yyyy-MM-dd", arabicCulture),
                ArrivalTime = RightToLeftCharacter + TripModel.ArriveTime.ToString("hh:mm tt", arabicCulture),
                ArrivalDay = TripModel.ArriveTime.ToString("dddd", arabicCulture),
                Avaliblility = TripModel.Avaliblility,
                Location = TripModel.Location,
                Price = TripModel.Price,
                CompanyName = TripModel.company.Name,
                BookedSeats = TripModel.bookings.SelectMany(b => b.seats.Where(s => s.State == SeatState.Booked).Select(s => s.Number)).ToList(),
                //ArriveTime = TripModel.ArriveTime.ToString("dddd yyyy-MM-dd hh:mm tt",arabicCulture),
                //Date = TripModel.Date.ToString("dddd yyyy-MM-dd",arabicCulture),
                //Time = TripModel.Time.ToString("hh:mm tt", arabicCulture),
            }).FirstOrDefaultAsync();
            return trip;
            // return trip == null ? NotFound() : Ok(trip);
        }

        public async Task<ApiResponse<TripDtos?>> AddTrip(int CompanyId, CreateTripDtos TripDtos)
        {
            var company = await _context.Companies.Include(c => c.Trips).FirstOrDefaultAsync(c => c.Id == CompanyId);

            if (company == null)
            {
                return new ApiResponse<TripDtos?>
                {
                    StatusCode = StatusCode.BadRequest,
                    Success = false,
                    message = "company not found "
                };
                //  return BadRequest("company not found");
            }
            var Tripmodel = TripDtos.ToTripFromCreateDtos();
            company.Trips.Add(Tripmodel);
            Tripmodel.CompanyId = company.Id;
            await _context.Trips.AddAsync(Tripmodel);
            await _context.SaveChangesAsync();


            var DepartureDateTime=Tripmodel.Date.ToDateTime(Tripmodel.Time);
            var reminderTime = DepartureDateTime.AddHours(-3);
           var jobId= BackgroundJob.Schedule<TripService>(service=>service.SendReminderEmail(Tripmodel.TripId),reminderTime);


            //_backgroundJobClient.Schedule<EmailService>(
            //    service => service.SendTripReminderEmails(trip.TripId),
            //    reminderTime);
            return new ApiResponse<TripDtos?>
            {
                StatusCode = StatusCode.Created,
                Success = true,
                message = "created success",
                Data = Tripmodel.ToTripDto()
            };
            //  return CreatedAtAction(nameof(GetById), new { Id = Tripmodel.TripId }, Tripmodel.ToTripDto());
        }

        public async Task<ApiResponse<string?>> DeleteTrip(int id)
        {
            try
            {
                var Tripmodel = await _context.Trips.FirstOrDefaultAsync(s => s.TripId == id);
                if (Tripmodel == null)
                {
                    return new ApiResponse<string?>
                    {
                        Success = false,
                        StatusCode = StatusCode.BadRequest,
                        message = "trip not found"
                    };
                    // return NotFound();
                }
                _context.Trips.Remove(Tripmodel);
                await _context.SaveChangesAsync();
                return new ApiResponse<string?>
                {
                    Success = true,
                    message = "Deleted successfully",
                    StatusCode = StatusCode.Ok,
                };
                // return NoContent();

            }
            catch (Exception ex)
            {
                return new ApiResponse<string?>
                {
                    StatusCode = StatusCode.BadRequest,
                    Success = false,
                    message = "error while deleting trip make sure it has no bookings and try again"
                };
                //return BadRequest(new ApiResponse<string>()
                //{
                //    Success = false,
                //    message = "error while deleting trip make sure it has no bookings and try again"
                //});
            }

        }
        public async Task<ApiResponse<TripDtos?>> EditTrip(int id, UpdateTripDtos UpdateDtos)
        {
            var Tripmodel = await _context.Trips.Include(t => t.company).FirstOrDefaultAsync(s => s.TripId == id);
            if (Tripmodel == null)
            {
                return new ApiResponse<TripDtos?>
                {
                    Success = false,
                    StatusCode = StatusCode.BadRequest,
                    message = "trip not found"
                };
                // return NotFound();
            }
            if (!string.IsNullOrEmpty(UpdateDtos.From)) Tripmodel.From = UpdateDtos.From;
            if (!string.IsNullOrEmpty(UpdateDtos.To)) Tripmodel.To = UpdateDtos.To;
            if (!string.IsNullOrEmpty(UpdateDtos.Class)) Tripmodel.Class = UpdateDtos.Class;
            if (!string.IsNullOrEmpty(UpdateDtos.Location)) Tripmodel.Location = UpdateDtos.Location;
            if (UpdateDtos.Price.HasValue) Tripmodel.Price = UpdateDtos.Price.Value;
            if (UpdateDtos.Date.HasValue) Tripmodel.Date = UpdateDtos.Date.Value;
            if (UpdateDtos.Time.HasValue) Tripmodel.Time = UpdateDtos.Time.Value;
            if (UpdateDtos.ArriveTime.HasValue) Tripmodel.ArriveTime = UpdateDtos.ArriveTime.Value;
            if (UpdateDtos.Avaliblility.HasValue) Tripmodel.Avaliblility = UpdateDtos.Avaliblility.Value;
            // if (UpdateDtos.TripCode.HasValue) Tripmodel.TripCode = UpdateDtos.TripCode.Value;
            await _context.SaveChangesAsync();
            return new ApiResponse<TripDtos?>
            {
                Success = true,
                StatusCode = StatusCode.Ok,
                message = "edit succeed",
                Data = Tripmodel.ToTripDto(),
            };
            //return Ok(Tripmodel.ToTripDto());
        }

        public async Task<IEnumerable<TripDtos>> GetCompanyTrips(int companyId)
        {
            var company = await _context.Companies.Include(c => c.Trips).AsNoTracking().FirstOrDefaultAsync(c => c.Id == companyId);
            if (company == null) return null;
            var trips = company.Trips.Select(trip => trip.ToTripDto());
            return trips;
        }

        public async Task<IEnumerable<TripDtos>?> Search(string? from, string? to, DateOnly? date)
        {
            var trips = await _context.Trips.Include(t => t.company).Where(s => (from == null || s.From == from) && (to == null || s.To == to) && (date == null || s.Date == date) && s.Avaliblility)
               .AsNoTracking()
               .Select(s => s.ToTripDto()).
               ToListAsync();
            return trips;
            // return trips.Count == 0 ? NotFound("لا توجد رحلات متاحة للمعايير المحددة.") : Ok(trips);
        }

        public async Task<IEnumerable<UserDetails>> GetPassengers(int TripId)
        {
           return await _context.Trips
                .Where(t => t.TripId == TripId)
                .SelectMany(t => t.bookings)
                .Select(b => new UserDetails()
                 {
                Id = b.UserId,
                FirstName = b.user.FirstName,
                LastName = b.user.LastName,
                Email = b.user.Email,
                phoneNumber = b.user.PhoneNumber,
                PhotoUrl = b.user.photo
            })
                .ToListAsync();
                 }

        public async Task<bool> SendReminderEmail(int TripId)
        {
            var passengers = await GetPassengers(TripId);
            foreach (var passenger in passengers)
            {
                var emailrequest = new EmailRequest
                {
                    Email = passenger.Email,
                    Subject = "تذكير بالرحلة",
                    Body = $",{passenger.FirstName}  باقي علي رحلتك س ساعات"
                };
                await _EmailService.SendEmail(emailrequest);
            }
            return true;
        }
    }
}


