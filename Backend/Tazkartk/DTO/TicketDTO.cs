namespace Tazkartk.DTO
{
    public class TicketDTO
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string userEmail { get; set; }
        public string CompanyName { get; set; }
        public string Date {  get; set; }
        public string Time {  get; set; }
        public string From { get; set; }
        public string To { get; set; }       
        public bool IsCanceled {  get; set; }
        public List<int> SeatsNumbers { get; set; }
      

      
    }
}
