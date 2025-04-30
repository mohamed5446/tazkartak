using System.ComponentModel.DataAnnotations;

namespace Tazkartk.DTO.TripDTOs
{
    public class CreateTripDtos
    {
        [Required]
        public string From { get; set; }
        [Required]
        public string To { get; set; }
        // public bool Avaliblility { get; set; } = true;
        [Required]
        public string Class { get; set; }

        public DateOnly Date { get; set; }
        [Required(ErrorMessage ="يجب ليب يس")]
        public TimeOnly Time { get; set; }

        //  public DateTime ArriveTime { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public double? Price { get; set; }
    }
}
