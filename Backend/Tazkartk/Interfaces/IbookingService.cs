using Tazkartk.DTO;

namespace Tazkartk.Interfaces
{
    public interface IbookingService
    {
        Task<bool> BookSeat(BookSeatDTO DTO);
        Task<List<BookingDetails>?> GetUserBookings(int userId);
        Task<List<BookingDetails>?> GetBookings();
        Task<bool> CancelSeat(int UserId, int BookingId);
        Task<bool> DeleteBooking(int Id);
    }
}
