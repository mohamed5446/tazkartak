using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tazkartk.DTO;
using Tazkartk.Email;
using Tazkartk.Helpers;
using Tazkartk.Interfaces;
using Tazkartk.Models;
using Tazkartk.Models.Enums;

namespace Tazkartk.Services
{
    public class AuthService:IAuthService
    {
        private readonly UserManager<Account> _AccountManager;
        private readonly JWT _jwt;
        private readonly IEmailService _EmailService;
        public AuthService(UserManager<Account> AccountManager ,IOptions<JWT> jwt,IEmailService emailService)
        {
            _AccountManager = AccountManager;
            _jwt = jwt.Value;
            _EmailService = emailService;

        }
        #region User
        public async Task<AuthModel> RegisterAsync(RegisterDTO DTO, Roles Role = Roles.User)
        {
            if (await _AccountManager.FindByEmailAsync(DTO.Email) != null)
            {
                return new AuthModel { message = "Email is already Registered", Success = false };
            }

            var user = new User
            {
                FirstName = DTO.FirstName,
                LastName = DTO.LastName,
                Email = DTO.Email,
                PhoneNumber = DTO.PhoneNumber,
                UserName = DTO.Email,

            };
            var result = await _AccountManager.CreateAsync(user, DTO.Password);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description} , ";
                }
                return new AuthModel { message = errors, Success = false };
            }

            await _AccountManager.AddToRoleAsync(user, Role.ToString());
            return await SendOTP(user.Email);
            //var JwtSecurityToken = await CreateJwtToken(user);
            //return new AuthModel
            //{
            //    Email = DTO.Email,
            //    ExpiresOn = JwtSecurityToken.ValidTo,
            //    isAuthenticated = true,
            //    Token = new JwtSecurityTokenHandler().WriteToken(JwtSecurityToken),
            //};
        }
        public async Task<AuthModel> LoginAsync(LoginDTO DTO)
        {
            var authModel = new AuthModel();

            var Account = await _AccountManager.FindByEmailAsync(DTO.Email);

            if (Account is null || !await _AccountManager.CheckPasswordAsync(Account, DTO.Password))
            {
                authModel.message = "Email or Password is incorrect!";
                return authModel;
            }
            if (!Account.EmailConfirmed)
            {
                authModel.message = "Email is not confirmed yet";
                return authModel;
            }

            var jwtSecurityToken = await CreateJwtToken(Account);

            authModel.isAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Email = Account.Email;
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;
            authModel.Roles = await _AccountManager.GetRolesAsync(Account);
            return authModel;
        }
        #endregion


        #region Company

        public async Task<AuthModel> CompanyRegisterAsync(CompanyRegisterDTO DTO)
        {
            if (await _AccountManager.FindByEmailAsync(DTO.Email) != null)
            {
                return new AuthModel { message = "Email is already Registered", Success = false };
            }
            var Company = new Company
            {
                Name = DTO.Name,
                Email = DTO.Email,
                PhoneNumber = DTO.Phone,
                UserName = DTO.Email,
                City = DTO.city,
                Street = DTO.street,
            };
            var result = await _AccountManager.CreateAsync(Company, DTO.Password);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description} , ";
                }
                return new AuthModel { message = errors, Success = false };
            }

            await _AccountManager.AddToRoleAsync(Company, Roles.Company.ToString());
            return await SendOTP(Company.Email);
        }
        #endregion


        #region OTP
        public async Task<AuthModel> SendOTP(string email)
        {
            var account = await _AccountManager.FindByEmailAsync(email);
            if (account == null)
            {
                return new AuthModel { message = "account not found " };
            }
            if (account.OTP != null && account.OTPExpiry > DateTime.UtcNow)
            {
                return new AuthModel { message = "there is already otp sent" };
            }
            string OTP = GenerateOTP();
            account.OTP = OTP;
            account.OTPExpiry = DateTime.UtcNow.AddMinutes(5);
            await _AccountManager.UpdateAsync(account);
            var emailrequest = new EmailRequest
            {
                Email = account.Email,
                Subject = "Your OTP",
                Body = $"Your OTP is {OTP}"
            };
            await _EmailService.SendEmail(emailrequest);
            return new AuthModel { message = "OTP has been sent to your email" };
        }
        public async Task<AuthModel> VerifyOtpAsync(string Email, string enteredOtp)
        {
            Account? Account = await _AccountManager.FindByEmailAsync(Email);
            if (Account == null)
            {
                return new AuthModel { message = "User not found" };
            }
            if (enteredOtp == null || enteredOtp != Account.OTP || DateTime.UtcNow > Account.OTPExpiry)
            {
                return new AuthModel { message = "Invalid OTP" };
            }

            Account.OTP = null;
            Account.OTPExpiry = null;
            Account.EmailConfirmed = true;
            await _AccountManager.UpdateAsync(Account);

            var JwtSecurityToken = await CreateJwtToken(Account);
            return new AuthModel
            {
                Email = Account.Email,
                ExpiresOn = JwtSecurityToken.ValidTo,
                isAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(JwtSecurityToken),
            };

            //return new AuthModel { message = "OTP verified successfully" };
        }
        private string GenerateOTP()
        {
            Random random = new Random();
            string randomNumber = random.Next(0, 1000000).ToString("D6");
            return randomNumber;
        }
        #endregion


        #region password
        public async Task<AuthModel> ForgetPassword(string Email)
        {
            var Account = await _AccountManager.FindByEmailAsync(Email);
            if (Account == null)
            {
                return new AuthModel { message = "user not found" };
            }

            return await SendOTP(Email);
        }
        public async Task<AuthModel> ChangePassword(ChangePasswordDTO DTO)
        {
            var Account = await _AccountManager.FindByEmailAsync(DTO.Email);
            if (Account == null)
            {
                return new AuthModel { message = "user not found" };
            }
            var checkpassword = await _AccountManager.CheckPasswordAsync(Account, DTO.OldPassword);
            if (!checkpassword)
            {
                return new AuthModel { message = "wrong password " };
            }
            var result = await _AccountManager.ChangePasswordAsync(Account, DTO.OldPassword, DTO.newPassword);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description} , ";
                }
                return new AuthModel { message = errors };
            }
            return new AuthModel { message = "Password changed successfully" };
        }
        public async Task<AuthModel> ResetPassword(ResetPasswordDTO DTO)
        {
            var Account = await _AccountManager.FindByEmailAsync(DTO.email);
            if (Account == null)
            {
                return new AuthModel { message = "user not found" };
            }
            if (DTO.token == null || DTO.token != Account.OTP || DateTime.UtcNow > Account.OTPExpiry)
            {
                return new AuthModel { message = "Invalid OTP" };
            }
            string NewHashedPassword = _AccountManager.PasswordHasher.HashPassword(Account, DTO.newPasswod);
            Account.PasswordHash = NewHashedPassword;
            Account.OTP = null;
            Account.OTPExpiry = null;
            var result = await _AccountManager.UpdateAsync(Account);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description} , ";
                }
                return new AuthModel { message = errors };
            }
            return new AuthModel { message = "Password Changed successfully" };
            //var result=await _UserManager.ResetPasswordAsync(user, token, NewPassword);
            //if(!result.Succeeded)
            //{
            //    var errors = string.Empty;
            //    foreach (var error in result.Errors)
            //    {
            //        errors += $"{error.Description} , ";
            //    }
            //    return new AuthModel { message = errors };

            // return new AuthModel { message = "Password reset successfully" };
        }
        #endregion



        #region JWT
        private async Task<JwtSecurityToken> CreateJwtToken(Account account)
        {
            var AccountClaims = await _AccountManager.GetClaimsAsync(account);
            var roles = await _AccountManager.GetRolesAsync(account);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, account.Email),
                new Claim("uid",account.Id.ToString()),
            }
            .Union(AccountClaims)
            .Union(roleClaims);


            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddHours(_jwt.DurationInMinutes),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
        #endregion



    }
}
