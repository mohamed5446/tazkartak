using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

using Microsoft.AspNetCore.Authorization;

using Tazkartk.Application.Interfaces;
using Tazkartk.Application.DTO;
namespace Tazkartk.API.Controllers
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
            var Booking = await _BookingService.GetBookingsAsync();
            return Booking == null ? NotFound() : Ok(Booking);
        }

        [HttpGet("/api/{userId}/tickets")]
        [SwaggerOperation(Summary = "List User Tickets")]
        public async Task<IActionResult> GetUserBookings(int userId)
        {
            var user = await _UserService.GetUserByIdAsync(userId);
            if (user == null) return NotFound();
            var bookings = await _BookingService.GetUserBookingsAsync(userId);
            return bookings == null ? NotFound() : Ok(bookings);
        }

        [HttpGet("/api/{userId}/History")]
        [SwaggerOperation(Summary = " User History")]
        public async Task<IActionResult> GetUserHistory(int userId)
        {
            var user = await _UserService.GetUserByIdAsync(userId);
            if (user == null) return NotFound();
            var bookings = await _BookingService.GetUserHistoryBookingsAsync(userId);
            return bookings == null ? NotFound() : Ok(bookings);
        }

        [HttpGet("GetTicket{Id}")]
        [SwaggerOperation(Summary = "Get Ticket By Id")]
        public async Task<IActionResult> GetTicket(int Id)
        {
            var ticket = await _BookingService.GetBookingByIdAsync(Id);
            return ticket == null ? NotFound() : Ok(ticket);
        }

        [HttpPost("{TripId}/guest")]
        [SwaggerOperation(Summary = "offline booking")]
        public async Task<IActionResult> OfflineBooking(int TripId, PassengerDTO DTO)
        {
            var result = await _BookingService.BookForGuestAsync(TripId, DTO);
            return StatusCode((int)result.StatusCode, result);
        }

        /// <summary>
        /// Book a ticket.
        /// </summary>
        /// <remarks>
        /// Test credentials:
        /// 
        /// card : 5123456789012346  
        /// expiry: 12/25  
        /// cvv: 123  
        /// wallet : 01010101010  
        /// mpin:123456
        /// otp: 123456
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> BookSeat(BookingDTO DTO)
        {
            var result = await _BookingService.BookSeatAsync(DTO);
            return StatusCode((int)result.StatusCode, result);
            // return Url== null ? NotFound() : Ok(new { url = Url });
        }

        [HttpPost("{BookingId}/cancel")]
        [SwaggerOperation(Summary = "Cancel a ticket")]
        public async Task<IActionResult> CancelBooking(int BookingId)
        {
            var result = await _BookingService.RefundAsync(BookingId);
            return StatusCode((int)result.StatusCode, result);
            //  return !result ? BadRequest() : Ok("refund requested"); 
        }

        [HttpPost("{TicketId}")]
        [SwaggerOperation(Summary = "Manual/offline Refund")]
        public async Task<IActionResult> ManaulRefund(int TicketId)
        {
            var result = await _BookingService.ManualRefund(TicketId);
            return StatusCode((int)result.StatusCode, result);
        }
        [HttpDelete("{TicketId}")]
        [SwaggerOperation(Summary = "delete ticket")]
        public async Task<IActionResult> DeleteBooking(int TicketId)
        {
            var result = await _BookingService.DeleteBookingAsync(TicketId);
            return StatusCode((int)result.StatusCode, result);
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

    }
}
