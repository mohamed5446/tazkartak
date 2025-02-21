using Tazkartk.Models;

namespace Tazkartk.DTO
{
    public class BookSeatDTO
    {
        public int UserId {  get; set; }
        public int TripId {  get; set; }
        public List<int> SeatsNumbers {  get; set; }
    }
}
