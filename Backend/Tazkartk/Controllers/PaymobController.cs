using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using Tazkartk.Application.Interfaces;
using Tazkartk.Application.DTO;

namespace Tazkartk.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymobController : ControllerBase
    {
        private readonly IPaymentService _PaymentService;
        public PaymobController( IPaymentService paymentService)
        {
            _PaymentService = paymentService;
        }
        [HttpPost("callback")]
        public async Task<IActionResult> CallBack()
        {
            var result = await _PaymentService.handleCallback(Request);
            return StatusCode((int)result.StatusCode, result);

            //using var reader = new StreamReader(Request.Body);
            //string body = await reader.ReadToEndAsync();
            //string receivedHmac = Request.Query["hmac"].ToString();

            //var callback = JsonConvert.DeserializeObject<paymobresponse>(body);

            //var valid = _PaymobService.ValidateHmac(callback, receivedHmac);
            //if (!valid)
            //{
            //    return Unauthorized("invalid hmac");
            //}
            //var obj = callback.obj;
            //var success = obj.success;
            //if (!obj.success)
            //{
            //    return BadRequest();
            //}
            //string transactionId = obj.id.ToString();
            //var paymentMethod = obj.source_data.sub_type;

            //if (obj.is_refunded)
            //{
            //    var Done = await _BookingService.CancelAsync(transactionId);
            //    if (!Done)
            //    {
            //        return BadRequest("failed to refund");
            //    }
            //    return Ok("refunded successfully");
            //}

            //var bookingDTO = new BookingDTO
            //{
            //    UserId = obj.payment_key_claims.extra.userid,
            //    TripId = obj.payment_key_claims.extra.tripid,
            //    SeatsNumbers = obj.payment_key_claims.extra.seats,
            //};
            //var done = await _BookingService.ConfirmBookingAsync(bookingDTO, transactionId, paymentMethod);
            //if (!done)
            //{
            //    return BadRequest();
            //}


            //return Ok();
        }

    }
}
