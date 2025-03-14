using Tazkartk.DTO;
using Tazkartk.DTO.Response;
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
        Task<List<TicketDTO>?> GetBookings();
        

        // Task<bool> CancelSeat(int UserId, int BookingId);
       
    }
}
