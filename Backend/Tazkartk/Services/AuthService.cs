using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Tazkartk.DTO;
using Tazkartk.DTO.AccontDTOs;
using Tazkartk.DTO.CompanyDTOs;
using Tazkartk.DTO.Response;
using Tazkartk.Email;
using Tazkartk.Helpers;
using Tazkartk.Interfaces;
using Tazkartk.Models;
using Tazkartk.Models.Enums;

namespace Tazkartk.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<Account> _AccountManager;
        private readonly JWT _jwt;
        private readonly IEmailService _EmailService;
        private readonly IConfiguration _conf;
        const string Pattern = "^[^@]+@[^@]+\\.[^@]+$";

        public AuthService(IConfiguration conf, UserManager<Account> AccountManager, IOptions<JWT> jwt, IEmailService emailService)
        {
            _AccountManager = AccountManager;
            _jwt = jwt.Value;
            _EmailService = emailService;
            _conf = conf;

        }


        #region User
        public async Task<AuthModel> RegisterAsync(RegisterDTO DTO, Roles Role = Roles.User)
        {
           
            bool match = Regex.IsMatch(DTO.Email, Pattern);
            if (!match)
            {
                return new AuthModel
                {
                    Success = false,
                    StatusCode = StatusCode.BadRequest,
                    message = "Invalid Email Address"
                };
            }
            if (await _AccountManager.FindByEmailAsync(DTO.Email) != null)
            {
                return new AuthModel
                {
                    Success = false,
                    StatusCode = StatusCode.BadRequest,
                    message = "Email is already registered",
                };
            }

            var user = new User
            {
                FirstName = DTO.FirstName,
                LastName = DTO.LastName,
                Email = DTO.Email,
                PhoneNumber = DTO.PhoneNumber,
                UserName = DTO.Email,
                photo = _conf["Avatar"]

            };
            var result = await _AccountManager.CreateAsync(user, DTO.Password);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description} , ";
                }
                return
                    new AuthModel
                    {
                        Success = false,
                        StatusCode = StatusCode.BadRequest,
                        message = errors
                    };
            }

            await _AccountManager.AddToRoleAsync(user, Role.ToString());
            return await SendOTP(user.Email);

        }
        public async Task<AuthModel> LoginAsync(LoginDTO DTO)
        {
            var Account = await _AccountManager.FindByEmailAsync(DTO.Email);

            if (Account is null || !await _AccountManager.CheckPasswordAsync(Account, DTO.Password))
            {
                return new AuthModel
                {
                    Success = false,
                    StatusCode = StatusCode.BadRequest,
                    message = "Email or password is incorrect "
                };
            }
            if (!Account.EmailConfirmed)
            {
                return new AuthModel
                {
                    Success = false,
                    StatusCode = StatusCode.BadRequest,
                    IsEmailConfirmed = Account.EmailConfirmed,
                    message = "please confirm your email",
                };
            }

            var jwtSecurityToken = await CreateJwtToken(Account);
            return new AuthModel
            {
                Success = true,
                StatusCode = StatusCode.Ok,
                Id = Account.Id,
                IsEmailConfirmed = Account.EmailConfirmed,
                isAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Email = Account.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                Roles = await _AccountManager.GetRolesAsync(Account),
            };
        }
        #endregion


        #region Company

        public async Task<AuthModel> CompanyRegisterAsync(CompanyRegisterDTO DTO)
        {
            bool match = Regex.IsMatch(DTO.Email, Pattern);
            if (!match)
            {
                return new AuthModel
                {
                    Success = false,
                    StatusCode = StatusCode.BadRequest,
                    message = "Invalid Email Address"
                };
            }
            if (await _AccountManager.FindByEmailAsync(DTO.Email) != null)
            {
                return new AuthModel
                {
                    Success = false,
                    StatusCode = StatusCode.BadRequest,
                    message = "Email is already Registered"
                };
            }
            var Company = new Company
            {
                Name = DTO.Name,
                Email = DTO.Email,
                PhoneNumber = DTO.Phone,
                UserName = DTO.Email,
                City = DTO.city,
                Street = DTO.street,
                Logo = _conf["Logo"]
            };
            var result = await _AccountManager.CreateAsync(Company, DTO.Password);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description} , ";
                }
                return new AuthModel
                {
                    Success = false,
                    StatusCode = StatusCode.BadRequest,
                    message = errors,
                };
            }
            await _AccountManager.AddToRoleAsync(Company, Roles.Company.ToString());
            return await SendOTP(Company.Email);
        }
        #endregion


        #region OTP
        public async Task<AuthModel> SendOTP(string email)
        {
            bool match = Regex.IsMatch(email, Pattern);
            if (!match)
            {
                return new AuthModel
                {
                    Success = false,
                    StatusCode = StatusCode.BadRequest,
                    message = "Invalid Email Address"
                };
            }
            var account = await _AccountManager.FindByEmailAsync(email);
            if (account == null)
            {
                return new AuthModel
                {
                    Success = false,
                    StatusCode = StatusCode.BadRequest,
                    message = "account not found "
                };
            }
            if (account.OTP != null && account.OTPExpiry > DateTime.UtcNow)
            {
                return new AuthModel
                {
                    Success = false,
                    StatusCode = StatusCode.BadRequest,
                    message = "there is already otp sent"
                };
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
            return new AuthModel
            {
                Success = true,
                StatusCode = StatusCode.Ok,
                Email = account.Email,
                message = "OTP has been sent to your email"
            };
        }
        public async Task<AuthModel> VerifyOtpAsync(string Email, string enteredOtp)
        {
            bool match = Regex.IsMatch(Email, Pattern);
            if (!match)
            {
                return new AuthModel
                {
                    Success = false,
                    StatusCode = StatusCode.BadRequest,
                    message = "Invalid Email Address"
                };
            }
            Account? Account = await _AccountManager.FindByEmailAsync(Email);
            if (Account == null)
            {
                return new AuthModel {
                    Success = false,
                    StatusCode = StatusCode.BadRequest,
                    message = "User not found"
                };
            }
            if (Account.EmailConfirmed)
            {
                return new AuthModel
                {
                    Success = false,
                    StatusCode = StatusCode.BadRequest,
                    message = "email is already confirmed",
                    IsEmailConfirmed = Account.EmailConfirmed
                };
            }
            if (enteredOtp == null || enteredOtp != Account.OTP || DateTime.UtcNow > Account.OTPExpiry)
            {
                return new AuthModel
                {
                    Success = false,
                    StatusCode = StatusCode.BadRequest,
                    message = "Invalid OTP",
                    IsEmailConfirmed = Account.EmailConfirmed };
            }

            Account.OTP = null;
            Account.OTPExpiry = null;
            Account.EmailConfirmed = true;
            await _AccountManager.UpdateAsync(Account);

            var JwtSecurityToken = await CreateJwtToken(Account);
            return new AuthModel
            {
                Success = true,
                StatusCode = StatusCode.Ok,
                Id = Account.Id,
                IsEmailConfirmed = Account.EmailConfirmed,
                Email = Account.Email,
                ExpiresOn = JwtSecurityToken.ValidTo,
                isAuthenticated = true,
                Roles = await _AccountManager.GetRolesAsync(Account),
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
            bool match = Regex.IsMatch(Email, Pattern);
            if (!match)
            {
                return new AuthModel
                {
                    Success = false,
                    StatusCode = StatusCode.BadRequest,
                    message = "Invalid Email Address"
                };
            }
            var Account = await _AccountManager.FindByEmailAsync(Email);
            if (Account == null)
            {
                return new AuthModel
                {
                    Success = false,
                    StatusCode = StatusCode.BadRequest,
                    message = "user not found"
                };
            }

            return await SendOTP(Email);
        }
        public async Task<AuthModel> ChangePassword(ChangePasswordDTO DTO)
        {
            var Account = await _AccountManager.FindByEmailAsync(DTO.Email);
            if (Account == null)
            {
                return new AuthModel
                {
                    Success = false,
                    StatusCode = StatusCode.BadRequest,
                    message = "user not found"
                };
            }
            var checkpassword = await _AccountManager.CheckPasswordAsync(Account, DTO.OldPassword);
            if (!checkpassword)
            {
                return new AuthModel
                {
                    Success = false,
                    StatusCode = StatusCode.BadRequest,
                    message = "wrong password "
                };
            }
            var result = await _AccountManager.ChangePasswordAsync(Account, DTO.OldPassword, DTO.newPassword);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description} , ";
                }
                return new AuthModel
                {
                    Success = false,
                    StatusCode = StatusCode.BadRequest,
                    message = errors
                };
            }
            return new AuthModel
            {
                Success = true,
                StatusCode = StatusCode.Ok,
                message = "Password changed successfully" 
            };
        }
    
        public async Task<AuthModel> ResetPassword(ResetPasswordDTO DTO)
        {
            bool match = Regex.IsMatch(DTO.email, Pattern);
            if (!match)
            {
                return new AuthModel
                {
                    Success = false,
                    StatusCode = StatusCode.BadRequest,
                    message = "Invalid Email Address"
                };
            }
            var Account = await _AccountManager.FindByEmailAsync(DTO.email);
            if (Account == null)
            {
                return new AuthModel
                {
                    Success = false,
                    StatusCode = StatusCode.BadRequest, 
                    message = "user not found"
                };
            }
            if (DTO.token == null || DTO.token != Account.OTP || DateTime.UtcNow > Account.OTPExpiry)
            {
                return new AuthModel 
                {
                    Success = false,
                    StatusCode = StatusCode.BadRequest, 
                    message = "Invalid OTP"
                };
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
                return new AuthModel 
                {
                    Success = false, 
                    StatusCode = StatusCode.BadRequest,
                    message = errors
                };
            }
            return new AuthModel
            { 
                Success=true,
                StatusCode=StatusCode.Ok,
                message = "Password Changed successfully"
            };
            //var result=await _UserManager.ResetPasswordAsync(user, token, NewPassword);
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
                expires: DateTime.Now.AddMinutes(_jwt.DurationInMinutes),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
        #endregion



    }
}
