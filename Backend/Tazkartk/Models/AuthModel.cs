namespace Tazkartk.Models
{
    public class AuthModel
    {
        public int? Id { get; set; }
        public string message { get; set; }
        public bool isAuthenticated { get; set; }
        public string? Email { get; set; }
        public string? Token { get; set; }
        public DateTime? ExpiresOn { get; set; }
        public bool Success { get; set; }=true;
        public IList<String>? Roles { get; set; }
        public bool IsEmailConfirmed {  get; set; }=false;
    }
}
