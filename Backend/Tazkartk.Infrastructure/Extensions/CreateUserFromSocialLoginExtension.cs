using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Extensions;
using Tazkartk.Application.DTO.Google;
using Tazkartk.Domain.Models;
using Tazkartk.Domain.Models.Enums;
using Tazkartk.Infrastructure.Data;
using Tazkartk.Infrastructure.Google;

namespace Tazkartk.Infrastructure.Extensions
{
    public static class CreateUserFromSocialLoginExtension
    {

        public static async Task<Account> CreateUserFromSocialLogin(this UserManager<Account> userManager, ApplicationDbContext context, CreateUserFromSocialLogin model, LoginProvider loginProvider)
        {
            var user = await userManager.FindByLoginAsync(loginProvider.GetDisplayName(), model.LoginProviderSubject);

            if (user is not null) return user;

            user = await userManager.FindByEmailAsync(model.Email);

            if (user is null)
            {
                user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    UserName = model.Email,
                    photo = model.ProfilePicture
                };

                await userManager.CreateAsync(user);
                await userManager.AddToRoleAsync(user, Roles.User.ToString());
                user.EmailConfirmed = true;
                await userManager.UpdateAsync(user);
                await context.SaveChangesAsync();
            }

            var userLoginInfo = new UserLoginInfo(loginProvider.GetDisplayName(), model.LoginProviderSubject, loginProvider.GetDisplayName().ToUpper());

            var result = await userManager.AddLoginAsync(user, userLoginInfo);

            if (result.Succeeded) return user;
            else return null;
        }
    }
}

