using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Tazkartk.DTO;
using Tazkartk.Interfaces;
using Tazkartk.Models;
using Tazkartk.Services;
using Microsoft.AspNetCore.Authorization;
namespace Tazkartk.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly IBookingService _BookingService;
        private readonly IUserService _UserService;


        public TicketsController(IBookingService bookingService, IUserService userService)
        {
            _BookingService = bookingService;
            _UserService = userService;
        }
        
        [HttpGet]
        //[Authorize(Roles = "Admin , Company")]
        [SwaggerOperation(Summary = "List All Tickets")]

        public async Task<IActionResult> GetBookings()
        {
            var Booking = await _BookingService.GetBookings();
            return Booking==null ? NotFound() : Ok(Booking);    
        }
        [HttpGet("/api/{userId}/tickets")]
        [SwaggerOperation(Summary = "List User Tickets")]

        public async Task<IActionResult> GetUserBookings(int userId)
        {
            var user = await _UserService.GetUserById(userId);
            if (user == null) return NotFound();
            var bookings = await _BookingService.GetUserBookings(userId);
            return bookings==null ? NotFound() : Ok(bookings);
        }
        [HttpGet("/api/{userId}/History")]
        [SwaggerOperation(Summary = " User History")]
        public async Task<IActionResult> GetUserHistory(int userId)
        {
            var user = await _UserService.GetUserById(userId);
            if (user == null) return NotFound();
            var bookings = await _BookingService.GetUserHistoryTickets(userId);
            return bookings == null ? NotFound() : Ok(bookings);
        }
        [HttpGet("GetTicket{Id}")]
        [SwaggerOperation(Summary = "Get Ticket By Id")]
        public async Task<IActionResult> GetTicket(int Id)
        {
            var users = await _BookingService.GetTicket(Id);
            return Ok(users);
        }
        //[HttpGet("/api/{userId}/Canceled")]
        //[SwaggerOperation(Summary = "List User Canceled Tickets")]
        //public async Task<IActionResult> GetUserCanceledTickets(int userId)
        //{
        //    var user = await _UserService.GetUserById(userId);
        //    if (user == null) return NotFound();
        //    var bookings = await _BookingService.GetUserCanceledTicekts(userId);
        //    return bookings == null ? NotFound() : Ok(bookings);
        //}

        [HttpPost("{TripId}/guest")]
        [SwaggerOperation(Summary = "offline booking")]

        public async Task<IActionResult> OfflineBooking(int TripId, PassengerDTO DTO)
        {
            var result = await _BookingService.BookForGuest(TripId, DTO);
            return StatusCode((int)result.StatusCode, result);
        }
        [HttpPost]
        [SwaggerOperation(Summary = "Book a Ticket")]

        public async Task<IActionResult> BookSeat(BookingDTO DTO)
        {
            var result = await _BookingService.BookSeat(DTO);
            return StatusCode((int)result.StatusCode, result);  
           // return Url== null ? NotFound() : Ok(new { url = Url });
        }
        [HttpPost("{BookingId}/cancel")]
        [SwaggerOperation(Summary = "Cancel a ticket")]

        public async Task<IActionResult> CancelBooking(int BookingId)
        {
            var result = await _BookingService.Refund(BookingId);
            return StatusCode((int)result.StatusCode, result);
          //  return !result ? BadRequest() : Ok("refund requested"); 
        }
        [HttpDelete("{TicketId}")]
        [SwaggerOperation(Summary = "force delete ticket")]
        public async Task<IActionResult>DeleteBooking(int TicketId)
        {
            var result =await _BookingService.DeleteBooking(TicketId);
            return StatusCode((int)result.StatusCode, result);
        }


    }
}
