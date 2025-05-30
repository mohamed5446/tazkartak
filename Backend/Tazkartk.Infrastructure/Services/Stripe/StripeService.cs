//using Microsoft.Extensions.Options;
//using Stripe;
//using Stripe.Checkout;
//using Tazkartk.Configurations;
//using Tazkartk.DTO;
//using Tazkartk.Helpers;
//using Tazkartk.Interfaces;

//namespace Tazkartk.Services
//{
//    using Tazkartk.Infrastructure.Stripe;

//    public class StripeService : IStripeService
//    {
//        private readonly StripeSettings _Stripe;

//        public StripeService(IOptions<StripeSettings> stripe)
//        {
//            _Stripe = stripe.Value;
//            StripeConfiguration.ApiKey = _Stripe.SecretKey;
//        }
//        public async Task<string> CreatePaymentIntentAsync(BookingDTO DTO, double total, int count)
//        {
//            var options = new SessionCreateOptions
//            {
//                PaymentMethodTypes = new List<string> { "card" },
//                LineItems = new List<SessionLineItemOptions>
//                {
//                new SessionLineItemOptions
//                {
//                    PriceData = new SessionLineItemPriceDataOptions
//                    {
//                        UnitAmount = (long)total * 100,
//                        Currency = "EGP",
//                        ProductData = new SessionLineItemPriceDataProductDataOptions
//                        {
//                            Name = $"Book {count} Seats  "
//                        }
//                    },
//                    Quantity = 1
//                }
//            },
//                Mode = "payment",
//                SuccessUrl = $"{_Stripe.redirection_url}",
//                CancelUrl = $"{_Stripe.redirection_url}",
//                Metadata = new Dictionary<string, string>
//                {
//                    { "UserId", DTO.UserId.ToString() },
//                    { "TripId", DTO.TripId.ToString() },
//                    { "SeatsNumbers", string.Join(",", DTO.SeatsNumbers) }
//                }
//            };

//            var service = new SessionService();
//            var session = await service.CreateAsync(options);
//            return session.Url;
//        }

//        public async Task<bool> RefundAsync(string PaymentIntentId)
//        {
//            var refundOptions = new RefundCreateOptions
//            {
//                PaymentIntent = PaymentIntentId
//            };
//            var refundService = new RefundService();
//            var refund = await refundService.CreateAsync(refundOptions);

//            if (refund.Status == "succeeded")
//            {
//                return true;
//            }
//            return false;
//        }
//    }
//}

