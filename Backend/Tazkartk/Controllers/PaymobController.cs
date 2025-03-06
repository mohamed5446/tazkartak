using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using Tazkartk.DTO;
using Tazkartk.Helpers;
using Tazkartk.Interfaces;

namespace Tazkartk.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymobController : ControllerBase
    {
        private readonly IBookingService _BookingService;
        private readonly PaymobSettings _Paymob;
        public PaymobController(IOptions<PaymobSettings> paymob, IBookingService bookingService)
        {
            _BookingService = bookingService;
            _Paymob = paymob.Value;
        }
        [HttpPost("callback")]
        public async Task<IActionResult> CallBack()
        {
            using var reader = new StreamReader(Request.Body);
            string body = await reader.ReadToEndAsync();
            JObject jsonObject = JObject.Parse(body);
            JObject obj = jsonObject["obj"] as JObject;
            string receivedHmac = Request.Query["hmac"].ToString();

            List<string> values = new List<string>
            {
              JsonConvert.SerializeObject(obj["amount_cents"]).Trim('"') ,
              JsonConvert.SerializeObject(obj["created_at"]).Trim('"') ,
               JsonConvert.SerializeObject(obj["currency"]).Trim('"') ,
                 JsonConvert.SerializeObject(obj["error_occured"]).Trim('"'),
                JsonConvert.SerializeObject(obj["has_parent_transaction"]).Trim('"') ,
               JsonConvert.SerializeObject(obj["id"]).Trim('"') ,
                 JsonConvert.SerializeObject(obj["integration_id"]).Trim('"') ,
               JsonConvert.SerializeObject(obj["is_3d_secure"]).Trim('"') ,
                JsonConvert.SerializeObject(obj["is_auth"]).Trim('"') ,
                JsonConvert.SerializeObject(obj["is_capture"]).Trim('"') ,
                 JsonConvert.SerializeObject(obj["is_refunded"]).Trim('"') ,
                JsonConvert.SerializeObject(obj["is_standalone_payment"]).Trim('"') ,
                 JsonConvert.SerializeObject(obj["is_voided"]).Trim('"') ,
                JsonConvert.SerializeObject(obj["order"]?["id"]).Trim('"') ,
                JsonConvert.SerializeObject(obj["owner"]).Trim('"') ,
                JsonConvert.SerializeObject(obj["pending"]).Trim('"') ,
                JsonConvert.SerializeObject(obj["source_data"]?["pan"]).Trim('"') ,
                 JsonConvert.SerializeObject(obj["source_data"]?["sub_type"]).Trim('"') ,
                JsonConvert.SerializeObject(obj["source_data"]?["type"]).Trim('"') ,
                JsonConvert.SerializeObject(obj["success"]).Trim('"')
            };
            string concatenatedString = string.Join("", values);
            string computedHmac = ComputeHmac(_Paymob.HMAC, concatenatedString);
            if (computedHmac != receivedHmac)
            {
                return Unauthorized(new { c = concatenatedString, computedhmac = computedHmac, message = "Invalid HMAC" });
            }
            string transactionId = obj["id"]?.ToString();
            var isrefunded = JsonConvert.SerializeObject(obj["is_refunded"]);
            var paymentMethod = JsonConvert.SerializeObject(obj["source_data"]?["sub_type"]).Trim('"');
            if (isrefunded == "true")
            {
                var done = await _BookingService.Cancel(transactionId);
                if (!done)
                {
                    return BadRequest("failed to refund");
                }
                return Ok("refunded successfully");

            }
            JToken extra = obj["payment_key_claims"]?["extra"];

            var bookingDTO = new BookSeatDTO
            {
                UserId = extra["userid"].Value<int>() ,
                TripId = extra["tripid"].Value<int>(),
                SeatsNumbers = extra["seats"]?.ToObject<List<int>>() 
            };

            var success = await _BookingService.ConfirmBooking(bookingDTO, transactionId,paymentMethod);
            if (!success)
            {
                return BadRequest();
            }

            return Ok(new { c = concatenatedString });
        }
        public static string ComputeHmac(string secret, string data)
        {
            byte[] key = Encoding.UTF8.GetBytes(secret);
            byte[] message = Encoding.UTF8.GetBytes(data);

            using (var hmac = new HMACSHA512(key))
            {
                byte[] hash = hmac.ComputeHash(message);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
