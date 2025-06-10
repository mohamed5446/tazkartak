namespace Tazkartk.Application.DTO
{
    public class PassengerDetailsDTO
    {
        public int TicketId { get; set; }
        public int? UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Email { get; set; }
        public string PhoneNumber { get; set; }
        public List<int> Seats { get; set; }
    }
}
