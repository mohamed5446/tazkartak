using Newtonsoft.Json;


namespace Tazkartk.Infrastructure.Services.PaymentGateway.Paymob.DTOs
{
    public class PaymentIntentResponseDTO
    {
        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }
    }
}
