using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using Tazkartk.Application.DTO;
using Tazkartk.Application.DTO.Payments;
using Tazkartk.Application.Interfaces.External;
using Tazkartk.Infrastructure.Helpers;
using Tazkartk.Infrastructure.Services.PaymentGateway.Paymob.DTOs;

namespace Tazkartk.Infrastructure.Services.PaymentGateway.Paymob
{
    public class PaymobService : IPaymentGateway
    {
        private readonly PaymobSettings _paymob;
        private readonly HttpClient _httpClient;
        private readonly ICachingService _cachingService;

        public PaymobService(IOptions<PaymobSettings> paymob, IHttpClientFactory httpClientFactory, ICachingService cachingService)
        {
            _paymob = paymob.Value;
            _httpClient = httpClientFactory.CreateClient();
            _cachingService = cachingService;
        }

        #region checkout
        public async Task<string> CreatePaymentIntentAsync(UserBookingDetails DTO, double amount)
        {
            var requestData = new
            {
                amount = amount * 100,
                currency = "EGP",
                payment_methods = new[] { "wallet", "card", "kiosk" },
                expiration = 300,
                billing_data = new
                {
                    first_name = DTO.firstName,
                    last_name = DTO.lastName,
                    phone_number = DTO.phoneNumber,
                    email = DTO.email,
                },
                _paymob.notification_url,/*"https://webhook.site/7937e293-5fac-489c-9bf9-851c00943044",*/ // "https://bfeb-156-207-25-225.ngrok-free.app/api/Paymob/callback",
                _paymob.redirection_url,  
                extras = new
                {
                    userid = DTO.userId,
                    tripid = DTO.tripId,
                    seats = DTO.seatNumbers

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
            string PaymentUrl = $"https://accept.paymob.com/unifiedcheckout/?publicKey={publicKey}&clientSecret={clientSecret}";

            return PaymentUrl;
        }

        #endregion

        #region refund
        public async Task<bool> RefundTransactionAsync(string transactionId, double amountCents)
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

        #endregion

        #region callback
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
            if (!success)
            {
                return new WebhookRequest
                {
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
                IsSuccessful = success,
                IsRefunded = is_refund,
                PaymentMethod = paymentMethod,
                TransactionId = transactionId,
                extra = bookingDTO,
            };
        }
        private bool ValidateHmac(paymobresponse callback, string recievedhmac)
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
        #endregion

        #region Payouts
        private async Task<TokenResponse> GenerateAccessToken()
        {
            var data = new[]
            {
                new KeyValuePair<string, string>("client_id",_paymob.Client_id ),
                new KeyValuePair<string, string>("client_secret",_paymob.Client_secret ),
                new KeyValuePair<string, string>("username", _paymob.Username),
                new KeyValuePair<string, string>("password",_paymob.Password ),
                new KeyValuePair<string, string>("grant_type", "password"),
            };
            var content = new FormUrlEncodedContent(data);

            var response = await _httpClient.PostAsync("https://stagingpayouts.paymobsolutions.com/api/secure/o/token/", content);
            if (!response.IsSuccessStatusCode)
                throw new Exception($"{response.StatusCode}: {response.ReasonPhrase}");

            var responseJson = await response.Content.ReadAsStringAsync();
            var tokendata = JsonConvert.DeserializeObject<TokenResponse>(responseJson);

            _cachingService.SetData("AccessToken", tokendata.AccessToken, tokendata.ExpiresIn / 60);
            _cachingService.SetData("RefreshToken", tokendata.RefreshToken);
            return tokendata;

        }
        public async Task<TokenResponse> RefreshAccessToken(string refreshToken)
        {
            var data = new[]
        {
            new KeyValuePair<string, string>("grant_type", "refresh_token"),
            new KeyValuePair<string, string>("refresh_token", refreshToken),
            new KeyValuePair<string, string>("client_id", _paymob.Client_id),
            new KeyValuePair<string, string>("client_secret", _paymob.Client_secret),
        };
            var content = new FormUrlEncodedContent(data);
            var response = await _httpClient.PostAsync("https://stagingpayouts.paymobsolutions.com/api/secure/o/token/", content);
            if (!response.IsSuccessStatusCode)
                throw new Exception($"{response.StatusCode}: {response.ReasonPhrase}");
            var responseJson = await response.Content.ReadAsStringAsync();
            var tokendata = JsonConvert.DeserializeObject<TokenResponse>(responseJson);
            _cachingService.SetData("AccessToken", tokendata.AccessToken, tokendata.ExpiresIn / 60);
            _cachingService.SetData("RefreshToken", tokendata.RefreshToken);
            return tokendata;

        }
        private async Task<string> GetAccessToken()
        {
            var AccessToken = _cachingService.GetData<string>("AccessToken");
            if (string.IsNullOrEmpty(AccessToken))
            {
                var refreshToken = _cachingService.GetData<string>("RefreshToken");

                if (!string.IsNullOrEmpty(refreshToken))
                {
                    var newTokenData = await RefreshAccessToken(refreshToken);
                    AccessToken = newTokenData.AccessToken;
                }
                else
                {
                    var newTokenData = await GenerateAccessToken();
                    AccessToken = newTokenData.AccessToken;
                }
            }
            return AccessToken;

        }
        public async Task<double> BalanceInquiry()
        {
            var AccessToken = await GetAccessToken();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://stagingpayouts.paymobsolutions.com/api/secure/budget/inquire/");


            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AccessToken);

            var response = await _httpClient.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"error happened: {responseString}");

            }
            var balance = JsonConvert.DeserializeObject<BalanceResponse>(responseString);
            return balance.CurrentBalance;
        }
        public async Task<dispurseresponse> DispurseAsync(string issuer, string walletnumber, double amount)
        {

            var AccessToken = await GetAccessToken();

            var requestData = new
            {
                amount,
                issuer,
                msisdn = walletnumber
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://stagingpayouts.paymobsolutions.com/api/secure/disburse/")
            {
                Content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json")
            };

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AccessToken);

            var response = await _httpClient.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($" error: {responseString}");

            }
            var disbursementResponse = JsonConvert.DeserializeObject<DisbursementResponse>(responseString);
            return new dispurseresponse
            {
                TransactionId = disbursementResponse.TransactionId,
                amount = disbursementResponse.Amount,
                Status = disbursementResponse.DisbursementStatus,
                Success = disbursementResponse.DisbursementStatus == "successful",
                message = disbursementResponse.StatusDescription,
            };

        }
        #endregion


        #region extras
        public async Task<string> GenerateAccessTokenn()
        {
            var data = new[]
            {
          new KeyValuePair<string, string>("client_id", "mkVByrVQFTsfYwC4FyxLfSutxDvl8p7UU2kCgNqk"),
          new KeyValuePair<string, string>("client_secret", "jcTE3AM7ydbTSrf5vJgMEdkJ1HPL7H4H7Xl5RO1zAnAUWergXAx5rFwP5qTrQ1EhkhE8bqkcSpWV6gKJVVVxhxysEz7uQMGGVUi0IQKDRZoQHnZ0P75nE67u5ePQGPI3"),
          new KeyValuePair<string, string>("username", "49_api_checker"),
          new KeyValuePair<string, string>("password", "pIB6dSKOAGrutR50XghWpIK#5"),
          new KeyValuePair<string, string>("grant_type", "password"),
      };
            var content = new FormUrlEncodedContent(data);

            var response = await _httpClient.PostAsync("https://stagingpayouts.paymobsolutions.com/api/secure/o/token/", content);
            if (!response.IsSuccessStatusCode)
                throw new Exception($"{response.StatusCode}: {response.ReasonPhrase}");

            var responseJson = await response.Content.ReadAsStringAsync();

            return responseJson;

        }
        #endregion
    }
}

