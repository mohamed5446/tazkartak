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
        public async Task<IActionResult> BookSeat(BookSeatDTO DTO)
        {
            var Url = await _BookingService.BookSeat(DTO);
            return Url== null ? NotFound() : Ok(new { url = Url });
        }
        [HttpPost("{BookingId}/cancel")]
        public async Task<IActionResult> CancelBooking(int BookingId)
        {
            var result = await _BookingService.Refund(BookingId);
            return !result ? NotFound() : Ok("refund requested"); 
        }
       
    }
}
