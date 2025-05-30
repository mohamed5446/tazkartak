using Tazkartk.Application.DTO.Google;
using Tazkartk.Domain.Models;

namespace Tazkartk.Application.Interfaces
{
    public interface IGoogleAuthService
    {
        Task<Account> GoogleSignIn(GooglesigninDTO model);
    }
}
