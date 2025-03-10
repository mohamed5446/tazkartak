using Tazkartk.Models.Enums;

namespace Tazkartk.DTO.Response
{
    public class AuthModel
    {
        public bool Success { get; set; } = true;
        public StatusCode StatusCode { get; set; }
        public string message { get; set; }
        public int? Id { get; set; }
        public string? Email { get; set; }
        public bool? IsEmailConfirmed { get; set; }
        public bool isAuthenticated { get; set; }
        public string? Token { get; set; }
        public DateTime? ExpiresOn { get; set; }
        public IList<string>? Roles { get; set; }


    }
}
