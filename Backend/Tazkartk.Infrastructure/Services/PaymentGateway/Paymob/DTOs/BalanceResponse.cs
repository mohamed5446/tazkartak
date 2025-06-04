using Newtonsoft.Json;

namespace Tazkartk.Infrastructure.Services.PaymentGateway.Paymob.DTOs
{
    public class BalanceResponse
    {
        [JsonProperty("current_balance")]
        public double CurrentBalance { get; set; }
    }
}
