//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Options;
//using Stripe;
//using Tazkartk.DTO;
//using Tazkartk.Helpers;
//using Tazkartk.Interfaces;

//namespace Tazkartk.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class StripeController : ControllerBase
//    {
//        private readonly IBookingService _BookingService;
//        private readonly StripeSettings _stripeSettings;
//        public StripeController(IBookingService bookingService, IOptions<StripeSettings> stripeSettings)
//        {
//            _BookingService = bookingService;
//            _stripeSettings = stripeSettings.Value;
//        }
//        [HttpPost("webhook")]
//        public async Task<IActionResult> StripeWebhook()
//        {
//            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

//            try
//            {
//                var stripeEvent = EventUtility.ConstructEvent(
//                    json,
//                    Request.Headers["Stripe-Signature"],
//                    _stripeSettings.stripe_signature,
//                   // "whsec_p8dwF1oVNFC8UhWxY2t1y4YcoMbXVyKT",
//                    throwOnApiVersionMismatch: false
//                // "whsec_98fd22e07e28cbbb23453aa0a4d844707090517d15787865abf7eba86b2a2ab4"

//                );
//                if (stripeEvent.Type == "charge.refunded")
//                {
//                    var charge = stripeEvent.Data.Object as Charge;
//                    var trxId = charge.PaymentIntentId;
//                    var done = await _BookingService.CancelAsync(trxId);
//                    if (!done)
//                    {
//                        return BadRequest("failed to refund");
//                    }
//                    return Ok("refunded successfully");
//                }

//                if (stripeEvent.Type == "checkout.session.completed")
//                {
//                    var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
//                    if (session != null)
//                    {
//                        var service = new PaymentIntentService();
//                        var paymentIntent = await service.GetAsync(session.PaymentIntentId);
//                        var userId = int.Parse(session.Metadata["UserId"]);
//                        var tripId = int.Parse(session.Metadata["TripId"]);
//                        var seatsNumbers = session.Metadata["SeatsNumbers"]
//                            .Split(',')
//                            .Select(int.Parse)
//                            .ToList();
//                        var paymentIntentId = session.PaymentIntentId;
//                        var paymentMethodId = paymentIntent.PaymentMethodId;
//                        var paymentMethodService = new PaymentMethodService();
//                        var paymentMethod = await paymentMethodService.GetAsync(paymentMethodId);

//                        BookingDTO BookingDTO = new BookingDTO();
//                        BookingDTO.UserId = userId;
//                        BookingDTO.TripId = tripId;
//                        BookingDTO.SeatsNumbers = seatsNumbers;
//                        var success = await _BookingService.ConfirmBookingAsync(BookingDTO, paymentIntentId,$"STR_{paymentMethod.Type}");
//                        if (!success)
//                        {
//                            return BadRequest("Failed to confirm booking.");
//                        }
//                    }
//                }
//                return Ok();
//            }
//            catch (StripeException ex)
//            {
//                return BadRequest($"{ex.Message}");
//            }
//        }
//    }
//}
