using Newtonsoft.Json;

namespace Tazkartk.DTO
{
    public class PaymentIntentResponseDTO
    {
        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }
    }
}
