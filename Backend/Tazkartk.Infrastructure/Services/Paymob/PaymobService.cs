using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Tazkartk.Application.DTO;
using Tazkartk.Application.DTO.Response;
using Tazkartk.Application.Interfaces;
using Tazkartk.Application.Services;
using Tazkartk.Infrastructure.Helpers;

namespace Tazkartk.Infrastructure.Paymob
{
    public class PaymobService :IPaymentGateway//IPaymobService
    {
        private readonly PaymobSettings _paymob;
        private readonly HttpClient _httpClient;

        public PaymobService(IOptions<PaymobSettings> paymob, IHttpClientFactory httpClientFactory)
        {
            _paymob = paymob.Value; 
            _httpClient = httpClientFactory.CreateClient();
        }
        //        public async Task<string> CreatePaymentIntentAsync(BookingDTO DTO, double amount, UserDetailsDTO UserDTO)

        public async Task<string> CreatePaymentIntentAsync(int userId,int tripId,List<int>seatNumbers, double amount, string firstname,string lastname,string PhoneNumber,string Email )
        {
            // string bookingInfoJson = JsonConvert.SerializeObject(DTO);
            var requestData = new
            {
                amount = amount * 100,
                currency = "EGP",
                payment_methods = new[] { "wallet", "card", "kiosk" },
                expiration = 300,
                billing_data = new
                {
                    first_name = firstname,
                    last_name = lastname,
                    phone_number = PhoneNumber,
                    email = Email,
                    //first_name = UserDTO.FirstName,
                    //last_name = UserDTO.LastName,
                    //phone_number = UserDTO.PhoneNumber,
                    //email = UserDTO.Email,
                },
                notification_url = _paymob.notification_url,/*"https://webhook.site/7937e293-5fac-489c-9bf9-851c00943044",*/ // "https://bfeb-156-207-25-225.ngrok-free.app/api/Paymob/callback",
                redirection_url = _paymob.redirection_url,  // "https://bfeb-156-207-25-225.ngrok-free.app/api/Paymob/callback",
                //_paymob.notification_url,//"https://webhook.site/7937e293-5fac-489c-9bf9-851c00943044", // "https://bfeb-156-207-25-225.ngrok-free.app/api/Paymob/callback",
                //_paymob.redirection_url,  // "https://bfeb-156-207-25-225.ngrok-free.app/api/Paymob/callback",
                extras = new
                {
                    userid = userId,
                    tripid = tripId,
                    seats = seatNumbers
                    //userid = DTO.UserId,
                    //tripid = DTO.TripId,
                    //seats = DTO.SeatsNumbers

                }

            };
            var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "https://accept.paymob.com/v1/intention/");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Token", _paymob.SecretKey);

            request.Content = content;
            var response = await _httpClient.SendAsync(request);

            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"error : {responseString}");
            }

            var responseData = JsonConvert.DeserializeObject<PaymentIntentResponseDTO>(responseString);
            string clientSecret = responseData.ClientSecret;
            string publicKey = _paymob.PublicKey;
            string checkoutUrl = $"https://accept.paymob.com/unifiedcheckout/?publicKey={publicKey}&clientSecret={clientSecret}";

            return checkoutUrl;
        }

        

        public async Task<WebhookRequest> HandleCallBack(HttpRequest Request)
        {
            using var reader = new StreamReader(Request.Body);
            string body = await reader.ReadToEndAsync();
            string receivedHmac = Request.Query["hmac"].ToString();

            var callback = JsonConvert.DeserializeObject<paymobresponse>(body);

            var valid = ValidateHmac(callback, receivedHmac);
            if (!valid)
            {
                throw new Exception("invalid hmac");
            }
            var obj = callback.obj;
            var success = obj.success;
            string transactionId = obj.id.ToString();
            var paymentMethod = obj.source_data.sub_type;
            var is_refund = obj.is_refunded;
            if(!success)
            {
                return new WebhookRequest {
                    IsSuccessful = false,
                };
            }    

            var bookingDTO = new BookingDTO
            {
                UserId = obj.payment_key_claims.extra.userid,
                TripId = obj.payment_key_claims.extra.tripid,
                SeatsNumbers = obj.payment_key_claims.extra.seats,
            };

            return new WebhookRequest
            {
                IsSuccessful=success,
                IsRefunded=is_refund,
                PaymentMethod = paymentMethod,
                TransactionId = transactionId,
                extra=bookingDTO,
                
                
            };
        }

        public async Task<bool> RefundTransactionAsync( string transactionId, double amountCents)
        {
            var requestData = new
            {
                transaction_id = transactionId,
                amount_cents = (int)amountCents,
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
                throw new Exception($"Refund error: {responseString}");

            }
            return true;
        }

        public bool ValidateHmac(paymobresponse callback, string recievedhmac)
        {
            var obj = callback.obj;
            var values = new List<string>
            {
                obj.amount_cents.ToString(),
                obj.created_at.ToString("yyyy-MM-ddTHH:mm:ss.ffffff"),
                obj.currency,
                obj.error_occured.ToString().ToLower(),
                obj.has_parent_transaction.ToString().ToLower(),
                obj.id.ToString(),
                obj.integration_id.ToString(),
                obj.is_3d_secure.ToString().ToLower(),
                obj.is_auth.ToString().ToLower(),
                obj.is_capture.ToString().ToLower(),
                obj.is_refunded.ToString().ToLower(),
                obj.is_standalone_payment.ToString().ToLower(),
                obj.is_voided.ToString().ToLower(),
                obj.order.id.ToString() ?? "",
                obj.owner.ToString(),
                obj.pending.ToString().ToLower(),
                obj.source_data?.pan ?? "",
                obj.source_data?.sub_type ?? "",
                obj.source_data?.type ?? "",
                obj.success.ToString().ToLower()
            };

            string concatenatedString = string.Join("", values);
            string computedHmac = ComputeHmac(_paymob.HMAC, concatenatedString);
            return computedHmac == recievedhmac;
        }
        private string ComputeHmac(string secret, string data)
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
