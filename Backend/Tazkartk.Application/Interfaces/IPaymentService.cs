using Microsoft.AspNetCore.Http;
using Tazkartk.Application.DTO;
using Tazkartk.Application.DTO.Response;

namespace Tazkartk.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<bool> TransferFunds(int TripId);
        Task<IReadOnlyList<PaymentDTO>> GetAllPaymentsAsync();
        Task<PaymentDTO> GetPaymentByIdAsync(int id);
        Task<ApiResponse<bool>> handleCallback(HttpRequest request);

    }
}
