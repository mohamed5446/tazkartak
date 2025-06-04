using Tazkartk.Domain.Models.Enums;

namespace Tazkartk.Application.DTO
{
    public class payoutDTO
    {
        public string Date { get; set; }
        public string amount { get; set; }
        public PaymentMethods PaymentMethod { get; set; }
        public string WalletNumber {  get; set; }


    }
}
