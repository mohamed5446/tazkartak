using Tazkartk.Models;
using Tazkartk.DTO.Response;

namespace Tazkartk.Google
{
    public interface IGoogleAuthService
    {
        Task<ApiResponse<Account>> GoogleSignIn(GooglesigninDTO model);
    }
}
