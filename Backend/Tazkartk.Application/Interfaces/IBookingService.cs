using System.Threading.Tasks;
using Tazkartk.Application.DTO;
using Tazkartk.Application.DTO.Response;

namespace Tazkartk.Application.Interfaces
{
    public interface IBookingService
    {
        Task<IReadOnlyList<TicketDTO>> GetBookingsAsync();
        Task<IReadOnlyList<TicketDTO>> GetUserBookingsAsync(int userId);
        Task<IReadOnlyList<TicketDTO>> GetUserHistoryBookingsAsync(int userId);
        Task<TicketDTO?> GetBookingByIdAsync(int id);
        Task<ApiResponse<string>> BookSeatAsync(BookingDTO DTO);
        Task<ApiResponse<string>> BookForGuestAsync(int TripId, PassengerDTO DTO);
        Task<bool> ConfirmBookingAsync(BookingDTO DTO, string PaymentIntentId, string PaymentMethod);
        Task<ApiResponse<bool>> RefundAsync(int bookingId);
        Task<bool> CancelAsync(string trxId);
        Task<ApiResponse<string>> ManualRefund(int Id);
        Task<ApiResponse<string>> DeleteBookingAsync(int Id);



    }
}
