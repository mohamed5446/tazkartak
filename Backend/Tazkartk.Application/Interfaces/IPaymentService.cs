using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Tazkartk.Application.DTO;
using Tazkartk.Application.DTO.Payments;
using Tazkartk.Application.DTO.Response;

namespace Tazkartk.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<bool> TransferFunds(int TripId);
        Task<IReadOnlyList<PaymentDTO>> GetAllPaymentsAsync();
        Task<PaymentDTO> GetPaymentByIdAsync(int id);
        Task<ApiResponse<bool>> handleCallback(HttpRequest request);
        // Task<string> Genrateaccesstoken();
        Task<double> BalanceInquiry();
        Task<dispurseresponse> DispurseAsync(string issuer, string walletnumber, double amount);

    }
}
