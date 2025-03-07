using Tazkartk.DTO;
using Tazkartk.DTO.UserDTOs;

namespace Tazkartk.Interfaces
{
    public interface IPaymobService
    {
        Task<string> CreatePaymentIntent(BookingDTO DTO, double amount, UserDetailsDTO UserDTO);
        Task<bool> RefundTransaction(int bookingId, string transactionId, double amountCents);
    }
}
