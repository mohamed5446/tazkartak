using Tazkartk.DTO;
using Tazkartk.Models;

namespace Tazkartk.Services
{
    public interface IAuthService
    {
        Task<AuthModel> RegisterAsync(RegisterDTO DTO);
        Task<AuthModel> LoginAsync(LoginDTO DTO);
        Task<AuthModel> CompanyRegisterAsync(CompanyRegisterDTO DTO);
        Task<AuthModel> CompanyLoginAsync(LoginDTO DTO);
        Task<AuthModel> VerifyOtpAsync(string Email, string enteredOtp);
        Task<AuthModel> SendOTP(string Email);
        Task<AuthModel> ForgetPassword(string Email);
        Task<AuthModel> ResetPassword(ResetPasswordDTO DTO);
        Task<AuthModel> ChangePassword(ChangePasswordDTO DTO);
        Task<AuthModel> AdminRegisterAsync(RegisterDTO model);

    }
}
