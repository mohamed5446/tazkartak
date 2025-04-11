using Tazkartk.DTO;
using Tazkartk.DTO.Response;
using Tazkartk.DTO.UserDTOs;
using Tazkartk.Models;
namespace Tazkartk.Interfaces
{
    public interface IBookingService
    {
        Task<ApiResponse<string?>> BookSeat(BookingDTO DTO);
        Task<bool> ConfirmBooking(BookingDTO DTO, string PaymentIntentId,string PaymentMethod);

        Task<ApiResponse<bool>> Refund(int bookingId);
        Task<bool> Cancel(string trxId);
        Task<List<TicketDTO>?> GetUserBookings(int userId);
        Task<List<TicketDTO>?> GetUserCanceledTicekts(int userId);
        Task<List<TicketDTO>?> GetUserHistoryTickets(int userId);
        Task<List<TicketDTO>?> GetBookings();
        Task<TicketDTO> GetTicket(int id);
        Task<ApiResponse<string>> DeleteBooking(int Id);
        Task<ApiResponse<string?>> BookForGuest(int TripId,PassengerDTO DTO);
        // Task<bool> CancelSeat(int UserId, int BookingId);

    }
}
