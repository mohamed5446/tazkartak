namespace Tazkartk.Application.DTO.TripDTOs
{
    public class UpdateTripDtos
    {
        public string? From { get; set; }
        public string? To { get; set; }
        public bool? Avaliblility { get; set; } = true;
        public string? Class { get; set; }
        public DateOnly? Date { get; set; }
        public TimeOnly? Time { get; set; }
        public string? Location { get; set; }
        public double? Price { get; set; }
    }
}
