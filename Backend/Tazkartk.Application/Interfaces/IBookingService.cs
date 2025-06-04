using Tazkartk.Application.DTO;
using Tazkartk.Application.DTO.Response;

namespace Tazkartk.Application.Interfaces
{
    public interface IBookingService
    {
        Task<ApiResponse<string>> BookSeatAsync(BookingDTO DTO);
        Task<bool> ConfirmBookingAsync(BookingDTO DTO, string PaymentIntentId, string PaymentMethod);

        Task<ApiResponse<bool>> RefundAsync(int bookingId);
        Task<bool> CancelAsync(string trxId);
        Task<IReadOnlyList<TicketDTO>> GetUserBookingsAsync(int userId);
        Task<IReadOnlyList<TicketDTO>> GetUserHistoryTicketsAsync(int userId);
        Task<IReadOnlyList<TicketDTO>> GetBookingsAsync();
        Task<TicketDTO?> GetTicketAsync(int id);
        Task<ApiResponse<string>> DeleteBookingAsync(int Id);
        Task<ApiResponse<string>> BookForGuestAsync(int TripId, PassengerDTO DTO);

        // Task<IReadOnlyList<TicketDTO>> GetUserCanceledTicektsAsync(int userId);

    }
}
