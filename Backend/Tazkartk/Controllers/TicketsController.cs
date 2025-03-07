using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tazkartk.DTO;
using Tazkartk.Interfaces;
using Tazkartk.Models;

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
        public async Task<IActionResult> GetBookings()
        {
            var Booking = await _BookingService.GetBookings();
            return Booking==null ? NotFound() : Ok(Booking);    
        }
        [HttpGet("/api/{userId}/tickets")]
        public async Task<IActionResult> GetUserBookings(int userId)
        {
            var user = await _UserService.GetUserById(userId);
            if (user == null) return NotFound();
            var bookings = await _BookingService.GetUserBookings(userId);
            return bookings==null ? NotFound() : Ok(bookings);
        }

        [HttpPost]
        public async Task<IActionResult> BookSeat(BookingDTO DTO)
        {
            var Url = await _BookingService.BookSeat(DTO);
            return Url== null ? NotFound() : Ok(new { url = Url });
        }
        [HttpPost("{BookingId}/cancel")]
        public async Task<IActionResult> CancelBooking(int BookingId)
        {
            var result = await _BookingService.Refund(BookingId);
            return !result ? BadRequest() : Ok("refund requested"); 
        }
        //osame edit
        //show specific one
        [HttpGet("/api/tickets/{ticketId}")]
        public async Task<IActionResult> GetTicket(int ticketId)
        {
            var booking = await _BookingService.GetBookingById(ticketId);
            if (booking == null) return NotFound("Ticket not found");

            var user = await _UserService.GetUserById(booking.UserId);
            if (user == null) return NotFound("User not found");

            var trip = booking.trip;
            if (trip == null) return NotFound("Trip details not found");

            var company = trip.company.Name;
            if (company == null) return NotFound("Company details not found");

            var payment = booking.payment;
            if (payment == null) return NotFound("Payment details not found");

            var ticketDetails = new
            {
                UserName = $"{user.FirstName} {user.LastName}",
                From = trip.From,
                To = trip.To,
                Day = trip.Date,
                Time = trip.Time,
                ArriveTime = trip.ArriveTime,
                CompanyName = trip.company.Name,
                Price = payment.amount
            };

            return Ok(ticketDetails);
        } //end showone
        //delete specific one
        [HttpDelete("/api/{userId}/tickets/{ticketId}")]
        public async Task<IActionResult> DeleteUserTicket(int userId, int ticketId)
        {
            var user = await _UserService.GetUserById(userId);
            if (user == null) return NotFound("User not found");

            var booking = await _BookingService.GetBookingById(ticketId);
            if (booking == null || booking.UserId != userId) return NotFound("Ticket not found");

            var result = await _BookingService.DeleteBooking(ticketId);
            if (!result) return BadRequest("Failed to delete ticket");

            return Ok("Ticket deleted successfully");
        }
        //end delete
    }
}

