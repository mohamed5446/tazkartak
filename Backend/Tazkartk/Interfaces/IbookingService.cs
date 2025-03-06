using Tazkartk.DTO;
using Tazkartk.Models;
namespace Tazkartk.Interfaces
{
    public interface IBookingService
    {
        Task<string?> BookSeat(BookSeatDTO DTO);
        Task<bool> ConfirmBooking(BookSeatDTO DTO, string PaymentIntentId,string PaymentMethod);

        Task<bool> Refund(int bookingId);
        Task<bool> Cancel(string trxId);
        Task<Booking> GetBookingById(int id);
        Task<List<TicketDTO>?> GetUserBookings(int userId);
        Task<List<TicketDTO>?> GetBookings();
        

        // Task<bool> CancelSeat(int UserId, int BookingId);
        Task<bool> DeleteBooking(int Id);
    }
}
