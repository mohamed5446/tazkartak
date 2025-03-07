using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tazkartk.Data;
using Tazkartk.DTO.TripDTOs;
using Tazkartk.Mappers;
using Tazkartk.Models;

namespace Tazkartk.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public TripsController(ApplicationDbContext context) {
        
        _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var trips = await _context.Trips.AsNoTracking().Select(trip=>trip.ToTripDto()).ToListAsync(); 
            if(trips==null)return NotFound();
            return Ok(trips);
           
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var trip = await _context.Trips.FindAsync(id);
            return trip == null ? NotFound() : Ok(trip.ToTripDto());
        }
        [HttpGet("Search")]
        public async Task<IActionResult> GetAll( string? from, string? to, DateOnly? date)
        {
            var trips = await _context.Trips.Where(s => (from == null || s.From == from) &&(to == null || s.To == to) && (date == null || s.Date == date) &&s.Avaliblility)
                .AsNoTracking()
                .Select(s => s.ToTripDto()).
                ToListAsync();

            return trips.Count == 0 ? NotFound("لا توجد رحلات متاحة للمعايير المحددة.") : Ok(trips);
        }
        [HttpGet("/api/{companyId}/Trips")]
        public async Task<IActionResult> GetCompanyTrips(int companyId)
        {
            var company = await _context.Companies.Include(c => c.Trips).AsNoTracking().FirstOrDefaultAsync(c => c.Id == companyId);
            if (company == null) return NotFound();
            var trips = company.Trips.Select(trip => trip.ToTripDto());
            return Ok(trips);

        }
        [HttpPost("{CompanyId}")]
        public async Task<IActionResult> Create(int CompanyId,CreateTripDtos TripDtos)
        {
            var company = await _context.Companies.Include(c=>c.Trips).FirstOrDefaultAsync(c=>c.Id==CompanyId);

            if (company == null) return BadRequest("company not found");
            var Tripmodel = TripDtos.ToTripFromCreateDtos();
           company.Trips.Add(Tripmodel);
            Tripmodel.CompanyId = company.Id;
            await _context.Trips.AddAsync(Tripmodel);
           await  _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById),new { Id = Tripmodel.TripId },Tripmodel.ToTripDto());

        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update( int id, UpdateTripDtos UpdateDtos)
        {
            var Tripmodel = await _context.Trips.FirstOrDefaultAsync(s => s.TripId == id);
            if (Tripmodel == null) 
            { 
            return NotFound();
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
            return Ok(Tripmodel.ToTripDto());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) 
        {        
        var Tripmodel= await _context.Trips.FirstOrDefaultAsync(s => s.TripId == id);
            if (Tripmodel==null) 
            {
                return NotFound();
            }
            _context.Trips.Remove(Tripmodel);
            await _context.SaveChangesAsync();
            return NoContent();
        }
       
    }
}
