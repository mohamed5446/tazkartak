using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using Tazkartk.DTO;
using Tazkartk.DTO.UserDTOs;
using Tazkartk.Helpers;
using Tazkartk.Interfaces;
using static System.Net.WebRequestMethods;

namespace Tazkartk.Services
{
    public class TapService : ITapService
    {
        private readonly PaymobSettings _paymob;
        private readonly HttpClient _httpClient;

        public TapService(IOptions<PaymobSettings> paymob, IHttpClientFactory httpClientFactory)
        {
            _paymob = paymob.Value;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<string> CreateTapChargeAsync(BookingDTO booking, double amount, UserDetailsDTO user)
        {
            var requestData = new
            {
                amount = amount,
                currency = "EGP",
                description = "حجز تذاكر",
                metadata = new
                {
                    user_id = booking.UserId,
                    trip_id = booking.TripId,
                    seats = string.Join(",", booking.SeatsNumbers)
                },
                reference = new
                {
                    transaction = $"txn_{booking.UserId}_{booking.TripId}",
                    order = $"ord_{Guid.NewGuid()}"
                },
                customer = new
                {
                    first_name = user.FirstName,
                    middle_name = "", // Optional
                    last_name = user.LastName,
                    email = user.Email,
                    phone = new
                    {
                        country_code = "20", // Make sure this is correct per user country
                        number = user.PhoneNumber
                    }
                },               
                source = new
                {
                    id = "src_all" // Use "src_card", "src_kw.knet", "token_id", etc. if needed
                },
                post = new
                {
                    url = "https://71ba-41-236-23-151.ngrok-free.app/api/Tap/Callback" // Webhook endpoint
                },
                redirect = new
                {
                    url = "https://tazkartak.vercel.app/profile/tickets" // _tapSettings.RedirectUrl // Where user is sent back after payment
                },
                platform = new
                {
                    name = "Tazkartk", // Optional: define your platform name
                    version = "1.0"
                },
              
            };

            var requestJson = JsonConvert.SerializeObject(requestData);
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.tap.company/v2/charges");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "sk_test_XKokBfNWv6FIYuTMg5sLPjhJ");
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Tap Charge Failed: {responseContent}");

            var responseData = JsonConvert.DeserializeObject<dynamic>(responseContent);
            string redirectUrl = responseData.transaction.url;

            return redirectUrl;
        }

        public async Task<bool> RefundTapTransactionAsync(string chargeId, int amount, string reason = "requested_by_customer")
        {
            // Round and convert amount to 2 decimal places as per EGP requirements
            int amountInCents = (int)(amount)*100; // For EGP

            var requestData = new
            {
                charge_id = chargeId,
                amount = amountInCents,
                currency = "EGP", // Adjust based on transaction
                reason = reason,  // Options: duplicate, fraudulent, requested_by_customer

                post = new
                {
                    url = "https://71ba-41-236-23-151.ngrok-free.app/paymob/callback" // Replace with your actual webhook
                },

                metadata = new
                {
                    initiated_by = "system",
                    source = "user_request"
                },

                reference = new
                {
                    merchant = $"refund_{DateTime.UtcNow.Ticks}"
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.tap.company/v2/refunds/")
            {
                Content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json")
            };

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "sk_test_XKokBfNWv6FIYuTMg5sLPjhJ");
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var error = JsonConvert.DeserializeObject<dynamic>(responseContent);
                throw new Exception($"Tap Refund Failed: {error?.message ?? responseContent}");
            }

            var responseData = JsonConvert.DeserializeObject<dynamic>(responseContent);
            string status = responseData.status;

            // Tap refund statuses include: INITIATED, PENDING, COMPLETED, FAILED
            return status == "INITIATED" || status == "PENDING";
        }


    }
}
