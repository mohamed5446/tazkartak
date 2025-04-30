using Tazkartk.Models.Enums;

namespace Tazkartk.DTO.Response
{
    public class AuthModel
    {
        public bool Success { get; set; }
        public StatusCode StatusCode { get; set; }
        public string message { get; set; }
        public int? Id { get; set; }
        public string? Email { get; set; }
        public bool? IsEmailConfirmed { get; set; }
        public bool? isAuthenticated { get; set; }
        public string? Token { get; set; }
        public DateTime? ExpiresOn { get; set; }
        public IList<string>? Roles { get; set; }
        public AuthModel()
        {
            
        }
        public AuthModel(bool success, string message, int? id, string? email, bool isAuthenticated, string? token, IList<string>? roles, DateTime? expiresOn, StatusCode statusCode)
        {
            Success = success;
            this.message = message;
            Id = id;
            Email = email;
            this.isAuthenticated = isAuthenticated;
            Token = token;
            Roles = roles;
            ExpiresOn = expiresOn;
            StatusCode = statusCode;
        }
        public AuthModel(bool success, string message, StatusCode statusCode)
        {
            Success = success;
            this.message = message;
            StatusCode = statusCode;
        }
        public AuthModel(bool success, string message, bool isEmailConfirmed, StatusCode statusCode)
        {
            Success  = success;
            this.message = message;
            IsEmailConfirmed = isEmailConfirmed;
            StatusCode = statusCode;
        }
        public AuthModel(bool success, string message, string email, StatusCode statusCode)
        {
            Success = success;
            this.message = message;
            Email=email;
            StatusCode = statusCode;
        }
       
        public static AuthModel Error(string message, StatusCode statusCode=StatusCode.BadRequest)
        {
            return new AuthModel(false, message, statusCode);
        }
        public static AuthModel Error( string message,  bool IsEmailConfirmed,StatusCode statusCode = StatusCode.BadRequest)
        {
            return new AuthModel(false, message, false,statusCode);
        }
        public static AuthModel Succeed(string message, StatusCode statuscode = StatusCode.Ok)
        {
            return new AuthModel(true,message, statuscode);
        }
        public static AuthModel Succeed(string message,int id,string email ,bool isauthenticated,string token, IList<string> roles, DateTime expireson,StatusCode statuscode=StatusCode.Ok)
        {
            return new AuthModel(true,message,id,email,isauthenticated,token,roles,expireson,statuscode);
        }
        public static AuthModel Succeed(string message,string email,StatusCode statuscode =StatusCode.Ok)
        {
            return new AuthModel (true,message,email,statuscode);
        }
    }
}
