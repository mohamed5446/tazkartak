using Tazkartk.Application;
using Tazkartk.Application.DTO.AccontDTOs;
using Tazkartk.Application.DTO.CompanyDTOs;
using Tazkartk.Application.DTO.Google;
using Tazkartk.Application.DTO.Response;

namespace Tazkartk.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthModel> SignInWithGoogleAsync(GooglesigninDTO model);
        Task<AuthModel> RegisterAsync(RegisterDTO DTO, Roles Role = Roles.User);
        Task<AuthModel> LoginAsync(LoginDTO DTO);
        Task<AuthModel> CompanyRegisterAsync(CompanyRegisterDTO DTO);
        // Task<AuthModel> CompanyLoginAsync(LoginDTO DTO);
        Task<AuthModel> VerifyOtpAsync(string Email, string enteredOtp);
        Task<AuthModel> SendOTP(string Email, bool IsReset = false);
        Task<AuthModel> ForgotPasswordAsync(string Email);
        Task<AuthModel> ResetPasswordAsync(ResetPasswordDTO DTO);
        Task<AuthModel> ChangePasswordAsync(ChangePasswordDTO DTO);
        //  Task<AuthModel> AdminRegisterAsync(RegisterDTO model);

    }
}
