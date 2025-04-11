using Tazkartk.DTO.AccontDTOs;
using Tazkartk.DTO.CompanyDTOs;
using Tazkartk.DTO.Response;
using Tazkartk.Models.Enums;

namespace Tazkartk.Interfaces
{
    public interface IAuthService
    {
        Task<AuthModel> RegisterAsync(RegisterDTO DTO,Roles Role=Roles.User);
        Task<AuthModel> LoginAsync(LoginDTO DTO);
        Task<AuthModel> CompanyRegisterAsync(CompanyRegisterDTO DTO);
       // Task<AuthModel> CompanyLoginAsync(LoginDTO DTO);
        Task<AuthModel> VerifyOtpAsync(string Email, string enteredOtp);
        Task<AuthModel> SendOTP(string Email,bool IsReset=false);
        Task<AuthModel> ForgetPassword(string Email);
        Task<AuthModel> ResetPassword(ResetPasswordDTO DTO);
        Task<AuthModel> ChangePassword(ChangePasswordDTO DTO);
      //  Task<AuthModel> AdminRegisterAsync(RegisterDTO model);

    }
}
