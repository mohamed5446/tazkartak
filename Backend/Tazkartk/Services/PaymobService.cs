using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using Tazkartk.DTO;
using Tazkartk.DTO.UserDTOs;
using Tazkartk.Helpers;
using Tazkartk.Interfaces;

namespace Tazkartk.Services
{
    public class PaymobService : IPaymobService
    {
        private readonly PaymobSettings _paymob;
        private readonly HttpClient _httpClient;

        public PaymobService(IOptions<PaymobSettings> paymob, IHttpClientFactory httpClientFactory)
        {
            _paymob = paymob.Value;
            _httpClient = httpClientFactory.CreateClient();
        }
        public async Task<string> CreatePaymentIntent(BookingDTO DTO, double amount, UserDetailsDTO UserDTO)
        {
            string bookingInfoJson = JsonConvert.SerializeObject(DTO);
            var requestData = new
            {
                amount = amount * 100,
                currency = "EGP",
                payment_methods = new[] { "wallet", "card" },
                expiration= 120,
                billing_data = new
                {
                    first_name = UserDTO.FirstName,
                    last_name = UserDTO.LastName,
                    phone_number = UserDTO.PhoneNumber,
                    email = UserDTO.Email,
                },
                notification_url = _paymob.notification_url, // "https://bfeb-156-207-25-225.ngrok-free.app/api/Paymob/callback",
                redirection_url = _paymob.redirection_url ,  // "https://bfeb-156-207-25-225.ngrok-free.app/api/Paymob/callback",
                extras = new
                {
                    userid = DTO.UserId,
                    tripid = DTO.TripId,
                    seats = DTO.SeatsNumbers
                }

            };
            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "https://accept.paymob.com/v1/intention/");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Token", _paymob.SecretKey);

            request.Content = content;
            var response = await _httpClient.SendAsync(request);

            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"error : {responseString}");
            }

            var responseData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseString);
            string clientSecret = responseData.client_secret;
            string publicKey = _paymob.PublicKey;
            string checkoutUrl = $"https://accept.paymob.com/unifiedcheckout/?publicKey={publicKey}&clientSecret={clientSecret}";

            return checkoutUrl;
        }

        public async Task<bool> RefundTransaction(int bookingId, string transactionId, double amountCents)
        {
            var requestData = new
            {
                transaction_id = transactionId,
                amount_cents = (int)(amountCents),

            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://accept.paymob.com/api/acceptance/void_refund/refund")
            {
                Content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json")
            };

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Token", _paymob.SecretKey);

            var response = await _httpClient.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var errorData = JsonConvert.DeserializeObject<dynamic>(responseString);

                throw new Exception($"Refund Failed: {errorData.message}");
            }

            var responseData = JsonConvert.DeserializeObject<dynamic>(responseString);
            if (!(bool)responseData.success)
            {
                throw new Exception($"Refund Failed: {responseData.message}");
            }
            return(bool) responseData.success == true;
        }
    }
}
