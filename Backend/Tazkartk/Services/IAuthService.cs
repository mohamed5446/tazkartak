using Tazkartk.DTO;
using Tazkartk.Models;

namespace Tazkartk.Services
{
    public interface IAuthService
    {
        Task<AuthModel> RegisterAsync(RegisterDTO userRegisterDTO);
        Task<AuthModel> LoginAsync(LoginDTO loginDTO);
        Task<AuthModel> CompanyRegisterAsync(CompanyRegisterDTO companyRegisterDTO);
        Task<AuthModel> CompanyLoginAsync(LoginDTO loginDTO);
    }
}
