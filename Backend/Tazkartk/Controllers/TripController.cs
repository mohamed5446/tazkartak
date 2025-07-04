﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.Design;
using System.Text.Json;
using Tazkartk.Application.DTO;
using Tazkartk.Application.DTO.TripDTOs;
using Tazkartk.Application.Interfaces;

namespace Tazkartk.API.Controllers
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
        [HttpPost("{CompanyId}/Import")]
        [SwaggerOperation(Summary = "Import Trips from Excel")]
        public async Task<IActionResult> ImportTripsFromExcel(int CompanyId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("الملف غير موجود أو فارغ.");
            var trips = await _TripService.ImportFromExcelAsync(CompanyId, file);
            return Ok(trips);
        }
       
        [HttpGet]
        [SwaggerOperation(Summary = "List All Trips")]
        public async Task<IActionResult> GetTrips()
        {
            var Trips = await _TripService.GetTripsAsync();
            return Trips == null ? NotFound() : Ok(Trips);
        }
        [HttpGet("Available")]
        // [Authorize(Roles = "Admin , Company")]
        [SwaggerOperation(Summary = "List All Available Trips")]
        public async Task<IActionResult> GetAvailableTrips()
        {
            var Trips = await _TripService.GetAvailableTripsAsync();
            return Trips == null ? NotFound() : Ok(Trips);
        }
       
        [HttpGet("/api/{companyId}/Trips")]
        //  [Authorize(Roles = "Admin , Company")]
        [SwaggerOperation(Summary = "List Company Trips")]
        public async Task<IActionResult> GetCompanyTrips(int companyId)
        {
            var Trips = await _TripService.GetCompanyTripsAsync(companyId);
            return Trips == null ? NotFound() : Ok(Trips);
        }

        [HttpGet("Search")]
        [SwaggerOperation(Summary = "Search Trips")]
        public async Task<IActionResult> GetAll(string? from, string? to, DateOnly? date)
        {
            var Trips = await _TripService.SearchAsync(from, to, date);
            return Trips == null ? NotFound() : Ok(Trips);
        }
        [HttpGet("{TripId}/Passengers")]
        [SwaggerOperation(Summary = "List Trip Passengers")]

        public async Task<IActionResult> GetPassengers(int TripId)
        {
            var users = await _TripService.GetPassengersAsync(TripId);
            return Ok(users);
        }

        // [Authorize(Roles = "Admin , Company")]       
        [HttpGet("{Id:int}")]
        [SwaggerOperation(Summary = "Get Trip By Id")]

        public async Task<IActionResult> GetTrip(int Id)
        {
            var Trip = await _TripService.GetTripByIdAsync(Id);
            return Trip == null ? NotFound("Trip Not Found") : Ok(Trip);
        }
        [HttpPost("{CompanyId}")]
        // [Authorize(Roles = "Admin , Company")]
        [SwaggerOperation(Summary = "Add Trip")]

        public async Task<IActionResult> CreateTrip(int CompanyId, CreateTripDtos DTO)
        {
            var result = await _TripService.AddTripAsync(CompanyId, DTO);
            return StatusCode((int)result.StatusCode, result);
        }


        [HttpPut("{Id:int}")]
        // [Authorize(Roles = "Admin , Company")]
        [SwaggerOperation(Summary = "Edit Trip")]
        //[Authorize]
        public async Task<IActionResult> EditTrip(int Id, UpdateTripDtos DTO)
        {
            var result = await _TripService.EditTripAsync(Id, DTO);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpDelete("{Id:int}")]
        // [Authorize(Roles = "Admin , Company")]
        [SwaggerOperation(Summary = "Delete Trip")]
        public async Task<IActionResult> DeleteTrip(int Id)
        {
            var result = await _TripService.DeleteTripAsync(Id);
            return StatusCode((int)result.StatusCode, result);
        }

        #region Extras
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
        //[HttpDelete("{jobId}/delete job")]
        //public async Task<IActionResult> DeleteScheduled(string jobId)
        //{
        //    _TripService.deletejob(jobId);
        //    return Ok();
        //}
        //[HttpDelete("{TripId}/delete jobs")]
        //public async Task<IActionResult> DeleteScheduled(int TripId)
        //{
        //    _TripService.deletejobs(TripId);
        //    return Ok();
        //}
        //[HttpPost("markunavailable/{TripId}")]
        //public async Task<IActionResult> MarkTripUnavailable(int TripId)
        //{
        //    var res=await _TripService.MarkTripUnavailableAsync(TripId);
        //    return Ok(res);
        //}
        //[HttpGet("{TripId}/Bookings")]
        //public async Task<IActionResult>GetTripBookings(int TripId)
        //{
        //    var trips=await _TripService.GetBookingsByTrip(TripId);
        //    return Ok(trips);
        //}
        //[HttpPost("{TripId}/Send_Email")]
        //public async Task<IActionResult> SendEmail(int TripId, EmailDTO DTO)
        //{
        //    var result = await _TripService.send_Email_to_passengersAsync(TripId, DTO);
        //    return Ok(result);
        //}
        #endregion
    }
}
