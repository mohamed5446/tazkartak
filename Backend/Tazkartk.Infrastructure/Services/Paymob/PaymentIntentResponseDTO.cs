using Newtonsoft.Json;


namespace Tazkartk.Infrastructure.Paymob
{
    internal class PaymentIntentResponseDTO
    {
        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }
    }
}
