namespace Tazkartk.DTO
{
    public class UpdateTripDtos
    {
        public string From { get; set; }
        public string To { get; set; }
        public bool Avaliblility { get; set; } = true;
        public string Class { get; set; }
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
        public DateTime ArriveTime { get; set; }
        public string Location { get; set; }
        public Double Price { get; set; }
    }
}
