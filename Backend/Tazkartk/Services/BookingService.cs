using Microsoft.EntityFrameworkCore;
using Tazkartk.DTO;
using Tazkartk.Interfaces;
using Tazkartk.Models.Enums;
using Tazkartk.Models;
using Tazkartk.Data;

namespace Tazkartk.Services
{
    public class BookingService : IbookingService
    {
        private readonly ApplicationDbContext _context;

        public BookingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> BookSeat(BookSeatDTO DTO)
        {
            throw new NotImplementedException();

        }

        public Task<bool> CancelSeat(int UserId, int BookingId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteBooking(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<List<BookingDetails>?> GetBookings()
        {
            throw new NotImplementedException();
        }

        public Task<List<BookingDetails>?> GetUserBookings(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
