namespace Tazkartk.DTO
{
    public class CompanyEditDTO
    {
        public string? Name { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public IFormFile? Logo { get; set; }
        public string? PhoneNumber {  get; set; }
    }
}
