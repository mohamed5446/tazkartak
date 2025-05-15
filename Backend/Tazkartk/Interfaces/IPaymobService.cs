using Tazkartk.DTO;
using Tazkartk.DTO.UserDTOs;

namespace Tazkartk.Interfaces
{
    public interface IPaymobService
    {
        Task<string> CreatePaymentIntentAsync(BookingDTO DTO, double amount, UserDetailsDTO UserDTO);
        Task<bool> RefundTransactionAsync(int bookingId, string transactionId, double amountCents);
        public bool ValidateHmac(paymobresponse callback, string hmac);

    }
}
