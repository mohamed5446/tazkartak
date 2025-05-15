using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stripe;
using System.Text.Json;
using Tazkartk.DTO;
using Tazkartk.Interfaces;

namespace Tazkartk.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TapController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public TapController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost("Callback")]
        public async Task<IActionResult> CallBack()
        {
            using var reader = new StreamReader(Request.Body);
            string body = await reader.ReadToEndAsync();
            JObject json = JObject.Parse(body);

            var status = json["status"]?.ToString();
            if (status != "CAPTURED")
            {
                return BadRequest(new { message = "فشل الدفع أو لم يتم التأكيد", status });
            }
            var metadata = json["metadata"];
            if (metadata == null)
            {
                return BadRequest("Metadata مفقودة");
            }
            var seatsRaw = metadata["seats"]?.ToString(); // "3,5,6"
            var seats = seatsRaw?.Split(',').Select(int.Parse).ToList();
            var bookingDTO = new BookingDTO
            {
                UserId = metadata["user_id"].Value<int>(),
                TripId = metadata["trip_id"].Value<int>(),
                SeatsNumbers =seats,
            };
            var transactionId = json["id"]?.ToString();
            var paymentMethod = JsonConvert.SerializeObject(json["source"]?["payment_method"]).Trim('"');
            var done = await _bookingService.ConfirmBookingAsync(bookingDTO, transactionId, paymentMethod);
            if (!done)
            {
                return BadRequest(new { message = "فشل تأكيد الحجز" });
            }

            return Ok(new { success = true, message = "تم تأكيد الدفع بنجاح" });
        }

        
    }
}
