using Microsoft.AspNetCore.Identity;
using Tazkartk.Data;
using Tazkartk.Helpers;
using Tazkartk.DTO.Response;
using Microsoft.Extensions.Options;
using Hangfire.Logging;
using Tazkartk.Extensions;
using static Google.Apis.Auth.GoogleJsonWebSignature;
using Tazkartk.Models;

namespace Tazkartk.Google
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly UserManager<Account> _AccountManager;
        private readonly ApplicationDbContext _context;
        private readonly GoogleAuthSettings _googleAuthSettings;

        public GoogleAuthService(UserManager<Account> accountManager, ApplicationDbContext context, IOptions<GoogleAuthSettings> googleAuthSettings)
        {
            _AccountManager = accountManager;
            _context = context;
            _googleAuthSettings = googleAuthSettings.Value;
            
        }

        public async Task<ApiResponse<Account>> GoogleSignIn(GooglesigninDTO model)
        {
            Payload payload = new();

            try
            {
                payload = await ValidateAsync(model.IdToken, new ValidationSettings
                {
                    Audience = new[] { _googleAuthSettings.ClientId }
                });

            }
            catch (Exception ex)
            {
                return ApiResponse<Account>.Error("failed to validate");
            }

            var userToBeCreated = new CreateUserFromSocialLogin
            {
                FirstName = payload.GivenName,
                LastName = payload.FamilyName,
                Email = payload.Email,
                ProfilePicture = payload.Picture,
                LoginProviderSubject = payload.Subject,
            };

            var acc = await _AccountManager.CreateUserFromSocialLogin(_context, userToBeCreated, LoginProvider.Google);
           var user = acc as User;

            if (user is not null)
            {
                return ApiResponse<Account>.success(null, user);
            }

            else
            {
                return ApiResponse<Account>.Error("error happened");
            }
               
        }
    }
}
