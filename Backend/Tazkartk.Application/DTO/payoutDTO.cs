using Tazkartk.Domain.Models.Enums;

namespace Tazkartk.Application.DTO
{
    public class payoutDTO
    {
        public string PayoutId { get; set; }
        public string Date { get; set; }
        public string amount { get; set; }
        public string PaymentMethod { get; set; }
        public string WalletNumber {  get; set; }
        public string Status {  get; set; }


    }
}
