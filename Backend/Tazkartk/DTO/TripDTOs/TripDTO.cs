namespace Tazkartk.DTO.TripDTOs
{
    public class TripDTO
    {
        public int TripId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public bool Avaliblility { get; set; } = true;
        public string Class { get; set; }
        public string DepartureDate {  get; set; }
        public string DepartureTime {  get; set; }
        public string DepartureDay {  get; set; }
        public string ArrivalDate { get; set; }
        public string ArrivalTime { get; set; }
        public string ArrivalDay { get; set; }
        public string Location { get; set; }
        public double Price { get; set; }
        public string CompanyName { get; set; }
        public List<int>? BookedSeats { get; set; }

        //public string Date { get; set; }
        //public string Time { get; set; }
        //public string ArriveTime { get; set; }
    }
}
