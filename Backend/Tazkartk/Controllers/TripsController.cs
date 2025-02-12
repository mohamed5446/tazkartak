using Microsoft.AspNetCore.Mvc;
using Tazkartk.Data;
using Tazkartk.DTO;
//using Tazkartk.Dtos;
using Tazkartk.Mappers;

namespace WebApplication12.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public bool True { get; private set; }

        public TripsController(ApplicationDbContext context) {
        
        _context = context;
        }

        [HttpGet("Search")]
        public IActionResult GetAll([FromQuery] string from, [FromQuery] string to, [FromQuery] DateTime? date)
        {
            var trip = _context.Trips.ToList().
                Where(s => s.From == from && s.To == to && s.Date.Date == date.Value.Date && s.Avaliblility).
                Select(s => s.ToTripDto()).ToList();

            return trip.Count == 0 ? NotFound("لا توجد رحلات متاحة للمعايير المحددة.") : Ok(trip);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateTripDtos TripDtos)
        {
            var Tripmodel = TripDtos.ToTripFromCreateDtos();
            _context.Trips.Add(Tripmodel);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById),new { Id = Tripmodel.TripId },Tripmodel.ToTripDto());

        }
        [HttpPut]
        [Route("{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] UpdateTripDtos UpdateDtos)
        {
            var Tripmodel = _context.Trips.FirstOrDefault(s => s.TripId == id);
            if (Tripmodel == null) { 
            return NotFound();
            }
            Tripmodel.From = UpdateDtos.From;
            Tripmodel.To = UpdateDtos.To;
            Tripmodel.ArriveTime = UpdateDtos.ArriveTime;
            Tripmodel.Time = UpdateDtos.Time;
            Tripmodel.Price = UpdateDtos.Price;
            Tripmodel.Avaliblility = UpdateDtos.Avaliblility;
            Tripmodel.Class = UpdateDtos.Class;
            Tripmodel.Date= UpdateDtos.Date;
            Tripmodel.Location = UpdateDtos.Location;

            _context.SaveChanges();
            return Ok(Tripmodel.ToTripDto());

           
        }

        [HttpDelete]
        [Route("{id}")]
        public ActionResult Delete([FromRoute] int id) { 
        
        var Tripmodel= _context.Trips.FirstOrDefault(s => s.TripId == id);
            if (Tripmodel == null) {
                return NotFound();
            }

            _context.Trips.Remove(Tripmodel);
            _context.SaveChanges();
            return NoContent();
        }

        private object GetById()
        {
            throw new NotImplementedException();
        }
    }
}
