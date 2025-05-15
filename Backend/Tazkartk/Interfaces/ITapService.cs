using Tazkartk.DTO.UserDTOs;
using Tazkartk.DTO;

namespace Tazkartk.Interfaces
{
    public interface ITapService
    {
        Task<string> CreateTapChargeAsync(BookingDTO booking, double amount, UserDetailsDTO user);
        Task<bool> RefundTapTransactionAsync(string chargeId, int amount, string reason = "requested_by_customer");
    }
}
