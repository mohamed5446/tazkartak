using Tazkartk.DTO;
using Tazkartk.Models;

namespace Tazkartk.Interfaces
{
    public interface IPaymentService
    {
        Task<List<PaymentDTO>> GetAllPaymentsAsync();
        Task<PaymentDTO> GetPaymentByIdAsync(int id);
    }
}
