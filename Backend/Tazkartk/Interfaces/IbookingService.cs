using Tazkartk.DTO;
using Tazkartk.Models;
namespace Tazkartk.Interfaces
{
    public interface IBookingService
    {
        Task<string?> BookSeat(BookingDTO DTO);
        Task<bool> ConfirmBooking(BookingDTO DTO, string PaymentIntentId,string PaymentMethod);

        Task<bool> Refund(int bookingId);
        Task<bool> Cancel(string trxId);
        Task<List<TicketDTO>?> GetUserBookings(int userId);
        Task<List<TicketDTO>?> GetBookings();
        Task<Booking?> GetBookingById(int id);
        public Task<bool> DeleteBooking(int Id);

        // Task<bool> CancelSeat(int UserId, int BookingId);

    }
}
