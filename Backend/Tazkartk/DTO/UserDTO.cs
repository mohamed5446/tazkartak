namespace Tazkartk.DTO
{
    public class UserDTO
    {
        public string? firstName {  get; set; }
        public string? lastName { get; set; }
        public string? Phone { get; set; }

        public IFormFile? photo {  get; set; }
    }
}
