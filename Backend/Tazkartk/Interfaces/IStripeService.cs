using Tazkartk.DTO;

namespace Tazkartk.Interfaces
{
    public interface IStripeService
    {
        Task<string> CreatePaymentIntentAsync(BookingDTO DTO, double total, int count);
        Task<bool> RefundAsync(string PaymentIntentId);
    }
}
