using Tazkartk.DTO;
using Tazkartk.Models;

namespace Tazkartk.Interfaces
{
    public interface IPaymentService
    {
        Task<List<PaymentDTO>> GetAllPayments();
        Task<PaymentDTO> GetPaymentById(int id);
    }
}
