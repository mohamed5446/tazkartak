
namespace Tazkartk.Application.DTO
{
    public class BookingDTO
    {
        public int UserId { get; set; }
        public int TripId { get; set; }
        public List<int> SeatsNumbers { get; set; }
    }
}
