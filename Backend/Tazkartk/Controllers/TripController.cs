using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.Design;
using System.Text.Json;
using Tazkartk.DTO.AccontDTOs;
using Tazkartk.DTO.Response;
using Tazkartk.DTO.TripDTOs;
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
        
        //[HttpGet("Get-Passengers{TripId}")]
        //public async Task<IActionResult>GetPassengers(int TripId)
        //{
        //    var users = await _TripService.GetPassengers(TripId);
        //    return Ok(users);   
        //}

        [HttpGet]
        [SwaggerOperation(Summary = "List All Trips")]
        public async Task<IActionResult> GetTrips()
        {
            var Trips = await _TripService.GetTrips();
            return Trips == null ? NotFound() : Ok(Trips);
        }
        [HttpGet("/api/{companyId}/Trips")]
        [SwaggerOperation(Summary = "List Company Trips")]

        public async Task<IActionResult> GetCompanyTrips(int companyId)
        {
            var Trips=await _TripService.GetCompanyTrips(companyId);
            return Trips == null ? NotFound() : Ok(Trips);
        }
        [HttpGet("Search")]
        [SwaggerOperation(Summary = "Search Trips")]

        public async Task<IActionResult> GetAll(string? from, string? to, DateOnly? date)
        {
            var Trips = await _TripService.Search(from,to,date);
            return Trips == null ? NotFound() : Ok(Trips);
        }
        [HttpGet("{Id:int}")]
        [SwaggerOperation(Summary = "Get Trip By Id")]

        public async Task<IActionResult> GetTrip(int Id)
        {
            var Trip = await _TripService.GetTripById(Id);
            return Trip == null ? NotFound("Trip Not Found") : Ok(Trip);
        }
        [HttpPost("{CompanyId}")]
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
        [SwaggerOperation(Summary = "Delete Trip")]

        public async Task<IActionResult> DeleteTrip(int Id)
        {
            var result = await _TripService.DeleteTrip(Id);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
