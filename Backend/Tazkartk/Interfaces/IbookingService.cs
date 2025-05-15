using Microsoft.AspNetCore.Mvc;
using Tazkartk.DTO;
using Tazkartk.DTO.Response;
using Tazkartk.DTO.UserDTOs;
using Tazkartk.Models;
namespace Tazkartk.Interfaces
{
    public interface IBookingService
    {
        Task<ApiResponse<bool>> RefundTapAsync(int bookingId);
        Task<ApiResponse<string>> BookSeatAsync(BookingDTO DTO);
        Task<ApiResponse<string>> TapBookSeatAsync(BookingDTO DTO);
        Task<bool> ConfirmBookingAsync(BookingDTO DTO, string PaymentIntentId,string PaymentMethod);

        Task<ApiResponse<bool>> RefundAsync(int bookingId);
        Task<bool> CancelAsync(string trxId);
        Task<IReadOnlyList<TicketDTO>> GetUserBookingsAsync(int userId);
        Task<IReadOnlyList<TicketDTO>> GetUserCanceledTicektsAsync(int userId);
        Task<IReadOnlyList<TicketDTO>> GetUserHistoryTicketsAsync(int userId);
        Task<IReadOnlyList<TicketDTO>> GetBookingsAsync();
        Task<TicketDTO?> GetTicketAsync(int id);
        Task<ApiResponse<string>>DeleteBookingAsync(int Id);
        Task<ApiResponse<string>> BookForGuestAsync(int TripId,PassengerDTO DTO);

        // Task<bool> CancelSeat(int UserId, int BookingId);

    }
}
