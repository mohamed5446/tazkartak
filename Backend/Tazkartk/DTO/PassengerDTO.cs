using System.ComponentModel.DataAnnotations;

namespace Tazkartk.DTO
{
    public class PassengerDTO
    {
        public string Name {  get; set; }
        [Required, RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "Invalid Phone Number")]
        public string PhoneNumber { get; set; }
     //   public string PhotoUrl {  get; set; }
        public List<int> Seats { get; set; }
    }
}
