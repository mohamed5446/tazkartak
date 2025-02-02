using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tazkartk.DTO;
using Tazkartk.Email;
using Tazkartk.Helpers;
using Tazkartk.Models;
using Tazkartk.Models.Enums;

namespace Tazkartk.Services
{
    public class AuthService:IAuthService
    {
        private readonly UserManager<User> _UserManager;
        private readonly UserManager<Company> _CompanyManager;
        private readonly JWT _jwt;
        private readonly IEmailService _EmailService;
        public AuthService(UserManager<User> UserManager,UserManager<Company>CompanyManager ,IOptions<JWT> jwt,IEmailService emailService)
        {
            _UserManager = UserManager;
            _CompanyManager = CompanyManager;
            _jwt = jwt.Value;
            _EmailService = emailService;

        }
        #region User
        public async Task<AuthModel> LoginAsync(LoginDTO DTO)
        {
            var authModel = new AuthModel();

            var user = await _UserManager.FindByEmailAsync(DTO.Email);

            if (user is null || !await _UserManager.CheckPasswordAsync(user, DTO.Password))
            {
                authModel.message = "Email or Password is incorrect!";
                return authModel;
            }
            if (!user.EmailConfirmed)
            {
                authModel.message = "Email is not confirmed yet";
                return authModel;
            }

            var jwtSecurityToken = await CreateJwtToken(user);

            authModel.isAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Email = user.Email;
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;
            return authModel;
        }
        public async Task<AuthModel> RegisterAsync(RegisterDTO DTO)
        {
            if (await _UserManager.FindByEmailAsync(DTO.Email) != null)
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
            var result = await _UserManager.CreateAsync(user, DTO.Password);
            if (!result.Succeeded)
            {
               ;
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description} , ";
                }
                return new AuthModel { message = errors ,Success=false};
            }

            await _UserManager.AddToRoleAsync(user, Roles.User.ToString());
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
        #endregion


        #region Company
        public async Task<AuthModel> CompanyRegisterAsync(CompanyRegisterDTO DTO)
        {
            if (await _CompanyManager.FindByEmailAsync(DTO.Email) != null)
            {
                return new AuthModel { message = "Email is already Registered" };
            }
            if (await _CompanyManager.FindByEmailAsync(DTO.Email) != null)
            {
                return new AuthModel { message = "Email is already Registered" };
            }
            var company = new Company
            {
                Name = DTO.Name,
                Email = DTO.Email,
                PhoneNumber = DTO.Phone,
                UserName = DTO.Email,
                City = DTO.city,
                Street = DTO.street,


            };
            var result = await _CompanyManager.CreateAsync(company, DTO.Password);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description} , ";
                }
                return new AuthModel { message = errors };
            }
            //await _UserManager.AddToRoleAsync(user, Roles.User.ToString());
            var JwtSecurityToken = await CreateJwtToken(company);
            return new AuthModel
            {
                Email = DTO.Email,
                ExpiresOn = JwtSecurityToken.ValidTo,
                isAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(JwtSecurityToken),
            };
        }
        public async Task<AuthModel> CompanyLoginAsync(LoginDTO DTO)
        {
            var authModel = new AuthModel();

            var Company = await _CompanyManager.FindByEmailAsync(DTO.Email);

            if (Company is null || !await _CompanyManager.CheckPasswordAsync(Company, DTO.Password))
            {
                authModel.message = "Email or Password is incorrect!";
                return authModel;
            }

            var jwtSecurityToken = await CreateJwtToken(Company);

            authModel.isAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Email = Company.Email;
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;


            return authModel;

        }

        #endregion


        #region OTP
        public async Task<AuthModel> SendOTP(string email)
        {
            var user = await _UserManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new AuthModel { message = "user not found " };
            }
            if (user.OTP != null && user.OTPExpiry > DateTime.UtcNow)
            {
                return new AuthModel { message = "there is already otp sent" };
            }
            string OTP = GenerateOTP();
            user.OTP = OTP;
            user.OTPExpiry = DateTime.UtcNow.AddMinutes(5);
            await _UserManager.UpdateAsync(user);
            var emailrequest = new EmailRequest
            {
                Email = user.Email,
                Subject = "Your OTP",
                Body = $"Your OTP is {OTP}"
            };
            await _EmailService.SendEmail(emailrequest);
            return new AuthModel { message = "OTP has been sent to your email" };
        }
        public async Task<AuthModel> VerifyOtpAsync(string Email, string enteredOtp)
        {
            User? user = await _UserManager.FindByEmailAsync(Email);
            if (user == null)
            {
                return new AuthModel { message = "User not found" };
            }
            if (enteredOtp == null || enteredOtp != user.OTP || DateTime.UtcNow > user.OTPExpiry)
            {
                return new AuthModel { message = "Invalid OTP" };
            }

            user.OTP = null;
            user.OTPExpiry = null;
            user.EmailConfirmed = true;
            await _UserManager.UpdateAsync(user);

            var JwtSecurityToken = await CreateJwtToken(user);
            return new AuthModel
            {
                Email = user.Email,
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
            var user = await _UserManager.FindByEmailAsync(Email);
            if (user == null)
            {
                return new AuthModel { message = "user not found" };
            }

            return await SendOTP(Email);
        }
        public async Task<AuthModel> ChangePassword(ChangePasswordDTO DTO)
        {
            var user = await _UserManager.FindByEmailAsync(DTO.Email);
            if (user == null)
            {
                return new AuthModel { message = "user not found" };
            }
            var checkpassword = await _UserManager.CheckPasswordAsync(user, DTO.OldPassword);
            if (!checkpassword)
            {
                return new AuthModel { message = "wrong password " };
            }
            var result = await _UserManager.ChangePasswordAsync(user, DTO.OldPassword,DTO.newPassword);
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
            var user = await _UserManager.FindByEmailAsync(DTO.email);
            if (user == null)
            {
                return new AuthModel { message = "user not found" };
            }
            if (DTO.token == null || DTO.token != user.OTP || DateTime.UtcNow > user.OTPExpiry)
            {
                return new AuthModel { message = "Invalid OTP" };
            }
            string NewHashedPassword = _UserManager.PasswordHasher.HashPassword(user, DTO.newPasswod);
            user.PasswordHash = NewHashedPassword;
            user.OTP = null;
            user.OTPExpiry = null;
            var result = await _UserManager.UpdateAsync(user);
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

        public async Task<AuthModel> AdminRegisterAsync(RegisterDTO DTO)
        {
            if (await _UserManager.FindByEmailAsync(DTO.Email) != null)
            {
                return new AuthModel { message = "Email is already Registered" };
            }
            var user = new User
            {
                FirstName = DTO.FirstName,
                LastName = DTO.LastName,
                Email = DTO.Email,
                PhoneNumber = DTO.PhoneNumber,
                UserName = DTO.Email,

            };
            var result = await _UserManager.CreateAsync(user, DTO.Password);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description} , ";
                }
                return new AuthModel { message = errors };
            }

            await _UserManager.AddToRoleAsync(user, Roles.Admin.ToString());
            return await SendOTP(user.Email);
        }

        #region JWT
        private async Task<JwtSecurityToken> CreateJwtToken(User user)
        {
            var userClaims = await _UserManager.GetClaimsAsync(user);
            var roles = await _UserManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid",user.Id.ToString()),
            }
            .Union(userClaims)
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
        private async Task<JwtSecurityToken> CreateJwtToken(Company Company)
        {
            var CompanyClaims = await _CompanyManager.GetClaimsAsync(Company);
            //var roles = await _CompanyManager.GetRolesAsync(Company);
            //var roleClaims = new List<Claim>();

            //foreach (var role in roles)
            //    roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, Company.Email),
                new Claim("uid",Company.Id.ToString()),
            }
            .Union(CompanyClaims);

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
