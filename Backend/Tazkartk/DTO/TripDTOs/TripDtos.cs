namespace Tazkartk.DTO.TripDTOs
{
    public class TripDtos
    {
        public int TripId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public bool Avaliblility { get; set; } = true;
        public string Class { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string ArriveTime { get; set; }
        public string Location { get; set; }
        public double Price { get; set; }
        public string CompanyName {  get; set; }

    }
}
