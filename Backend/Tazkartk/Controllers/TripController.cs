using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.Design;
using System.Text.Json;
using Tazkartk.DTO;
using Tazkartk.DTO.AccontDTOs;
using Tazkartk.DTO.Response;
using Tazkartk.DTO.TripDTOs;
using Tazkartk.Email;
using Tazkartk.Interfaces;
using Tazkartk.Models;
using Tazkartk.Models.Enums;

namespace Tazkartk.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly ITripService _TripService;
        public TripsController(ITripService TripService)
        {
            _TripService = TripService;
        }
        //[HttpPut("transfer")]
        //public async Task<IActionResult>Transfer(int TripId)
        //{
        //    var result = await _TripService.transfer(TripId);
        //    if (result == true) return Ok();
        //    else return BadRequest();
        //}

        //[HttpPut("mark_unavailable/{TripId}")]
        //public async Task<IActionResult> MarkUnavailable(int TripId)
        //{
        // var changed=await _TripService.MarkTripUnavailable(TripId);
        //    if (changed) return Ok();
        //    else return BadRequest();
        //}
        //[HttpPost("Send trip reminder email({TripId}")]
        //public async Task<IActionResult> SendReminder(int TripId)
        //{
        //    var users = await _TripService.SendReminderEmail(TripId);
        //    return Ok();
        //}
        [HttpDelete("{TripId}/delete jobs")]
        public async Task<IActionResult> DeleteScheduled(int TripId)
        {
            _TripService.DeleteExistingJobs(TripId);
            return Ok();
        }
        [HttpGet]
        [SwaggerOperation(Summary = "List All Trips")]
        public async Task<IActionResult> GetTrips()
        {
            var Trips = await _TripService.GetTrips();
            return Trips == null ? NotFound() : Ok(Trips);
        }
        [HttpGet("Available")]
        // [Authorize(Roles = "Admin , Company")]
        [SwaggerOperation(Summary = "List All Available Trips")]
        public async Task<IActionResult> GetAvailableTrips()
        {
            var Trips = await _TripService.GetAvailableTrips();
            return Trips == null ? NotFound() : Ok(Trips);
        }
        //[HttpGet("{TripId}/Bookings")]
        //public async Task<IActionResult>GetTripBookings(int TripId)
        //{
        //    var trips=await _TripService.GetBookingsByTrip(TripId);
        //    return Ok(trips);
        //}
        [HttpGet("/api/{companyId}/Trips")]
        //  [Authorize(Roles = "Admin , Company")]
        [SwaggerOperation(Summary = "List Company Trips")]
        public async Task<IActionResult> GetCompanyTrips(int companyId)
        {
            var Trips = await _TripService.GetCompanyTrips(companyId);
            return Trips == null ? NotFound() : Ok(Trips);
        }

        [HttpGet("Search")]
        [SwaggerOperation(Summary = "Search Trips")]
        public async Task<IActionResult> GetAll(string? from, string? to, DateOnly? date)
        {
            var Trips = await _TripService.Search(from, to, date);
            return Trips == null ? NotFound() : Ok(Trips);
        }
        [HttpGet("{TripId}/Passengers")]
        [SwaggerOperation(Summary = "List Trip Passengers")]

        public async Task<IActionResult> GetPassengers(int TripId)
        {
            var users = await _TripService.GetPassengers(TripId);
            return Ok(users);
        }
       
       // [Authorize(Roles = "Admin , Company")]       
        [HttpGet("{Id:int}")]
        [SwaggerOperation(Summary = "Get Trip By Id")]

        public async Task<IActionResult> GetTrip(int Id)
        {
            var Trip = await _TripService.GetTripById(Id);
            return Trip == null ? NotFound("Trip Not Found") : Ok(Trip);
        }
        [HttpPost("{CompanyId}")]
       // [Authorize(Roles = "Admin , Company")]
        [SwaggerOperation(Summary = "Add Trip")]

        public async Task<IActionResult> CreateTrip(int CompanyId ,CreateTripDtos DTO)
        {
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState
                   .Where(m => m.Value.Errors.Any()).Where(m => !m.Key.Contains("DTO"))
                   .Select(m =>
                   {
                       var fieldName = m.Key.Split('.').Last();
                       return $" invalid {fieldName} format ";
                   })
                   .ToList();

                var errorMessage = string.Join("; ", errorMessages);

                return StatusCode(400, new ApiResponse<string>
                {
                    Success = false,
                    StatusCode = Models.Enums.StatusCode.BadRequest,
                    message = errorMessage
                });
            }
            var result = await _TripService.AddTrip(CompanyId,DTO);
            return StatusCode((int)result.StatusCode, result);
        }
       

        [HttpPut("{Id:int}")]
       // [Authorize(Roles = "Admin , Company")]
        [SwaggerOperation(Summary = "Edit Trip")]
        //[Authorize]
        public async Task<IActionResult> EditTrip(int Id, UpdateTripDtos DTO)
        {
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState
                    .Where(m => m.Value.Errors.Any()).Where(m => !m.Key.Contains("DTO"))
                    .Select(m =>
                    {       
                        var fieldName = m.Key.Split('.').Last();
                         return $" invalid {fieldName} format ";
                    })
                    .ToList();

                var errorMessage = string.Join("; ", errorMessages);

                return StatusCode(400, new ApiResponse<string>
                {
                    Success = false,
                    StatusCode = Models.Enums.StatusCode.BadRequest,
                    message = errorMessage
                });
            }

            var result = await _TripService.EditTrip(Id, DTO);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpDelete("{Id:int}")]
       // [Authorize(Roles = "Admin , Company")]
        [SwaggerOperation(Summary = "Delete Trip")]
        public async Task<IActionResult> DeleteTrip(int Id)
        {
            var result = await _TripService.DeleteTrip(Id);
            return StatusCode((int)result.StatusCode, result);
        }
        [HttpPost("{TripId}/Send_Email")]
        public async Task<IActionResult> SendEmail(int TripId, EmailDTO DTO)
        {
            var result = await _TripService.send_Email_to_passengers(TripId, DTO);
            return Ok(result);
        }
    }
}
