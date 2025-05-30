namespace Tazkartk.Application.DTO.CompanyDTOs
{
    public class CompanyDTO
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public double? Balance { get; set; }
        public string? Logo { get; set; }
    }
}
