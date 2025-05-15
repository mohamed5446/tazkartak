using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tazkartk.DTO.AccontDTOs;
using Tazkartk.DTO.CompanyDTOs;
using Tazkartk.DTO.Response;
using Tazkartk.Email;
using Tazkartk.Google;
using Tazkartk.Helpers;
using Tazkartk.Interfaces;
using Tazkartk.Models;
using Tazkartk.Models.Enums;
using AutoMapper;
using Tazkartk.Extensions;
using Tazkartk.Caching;

namespace Tazkartk.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<Account> _AccountManager;
        private readonly JWT _jwt;
        private readonly IEmailService _EmailService;
        private readonly IConfiguration _conf;
        private readonly IGoogleAuthService _googleAuthService;
        private readonly IMapper _mapper;
        private readonly ICachingService _cache;

        public AuthService(IConfiguration conf, UserManager<Account> AccountManager, IOptions<JWT> jwt, IEmailService emailService, IGoogleAuthService googleAuthService, IMapper mapper, ICachingService cachingService)
        {
            _AccountManager = AccountManager;
            _jwt = jwt.Value;
            _EmailService = emailService;
            _conf = conf;
            _googleAuthService = googleAuthService;
            _mapper = mapper;
            _cache = cachingService;
        }
        #region User
        public async Task<AuthModel> RegisterAsync(RegisterDTO DTO, Roles Role = Roles.User)
        {
          
            if (await _AccountManager.FindByEmailAsync(DTO.Email) != null)
            {
                return AuthModel.Error("البريد الإلكتروني مستخدم من قبل");
            }
            var user=_mapper.Map<User>(DTO);
            user.photo = _conf["Avatar"];
            user.UserName=user.Email;

            var result = await _AccountManager.CreateAsync(user, DTO.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(" , ", result.Errors.Select(e => e.Description));
                return AuthModel.Error(errors);
            }

            await _AccountManager.AddToRoleAsync(user, Role.ToString());
            return await SendOTP(user.Email);

        }
        public async Task<AuthModel> LoginAsync(LoginDTO DTO)
        {
            var Account = await _AccountManager.FindByEmailAsync(DTO.Email);

            if (Account is null || !await _AccountManager.CheckPasswordAsync(Account, DTO.Password))
            {
                return AuthModel.Error("يرجى التحقق من البريد الإلكتروني أو كلمة المرور.");
            }
            if (!Account.EmailConfirmed)
            {
                return AuthModel.Error("يرجى تأكيد البريد الإلكتروني أولاً", false);
            }

            var jwtSecurityToken = await CreateJwtToken(Account);
            //    return AuthModel.Succeed("authed", Account.Id, Account.Email, true, new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken), await _AccountManager.GetRolesAsync(Account), jwtSecurityToken.ValidTo);
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
            if (await _AccountManager.FindByEmailAsync(DTO.Email) != null)
            {
                return AuthModel.Error("البريد الإلكتروني مستخدم من قبل");    
            }
            var Company=_mapper.Map<Company>(DTO);
            Company.Logo = _conf["Logo"];
            Company.UserName=Company.Email;
            var result = await _AccountManager.CreateAsync(Company, DTO.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(" , ", result.Errors.Select(e => e.Description));
                return AuthModel.Error(errors);
            }
            await _AccountManager.AddToRoleAsync(Company, Roles.Company.ToString());
            return await SendOTP(Company.Email);
        }
        #endregion


        #region OTP
        public async Task<AuthModel> SendOTP(string email,bool IsReset=false)
        {
            var account = await _AccountManager.FindByEmailAsync(email);
            if (account == null)
            {
               return AuthModel.Error("لم يتم العثور على حساب "); 
            }
            var key = $"OTP_{email}";
            var existingOtp = _cache.GetData<string>(key);
            if(existingOtp != null) 
            {
                return AuthModel.Error("يوجد رمز تحقق تم إرساله بالفعل");
            }
            //if (account.OTP != null && account.OTPExpiry > DateTime.UtcNow)
            //{
            //  return AuthModel.Error("يوجد رمز تحقق تم إرساله بالفعل");
            //}
            string OTP = GenerateOTP();
            _cache.SetData<string>(key, OTP,5);
            //account.OTP = OTP;
            //account.OTPExpiry = DateTime.UtcNow.AddMinutes(5);
            await _AccountManager.UpdateAsync(account);
            var emailrequest = new EmailRequest
            {
                Email = account.Email,
                Subject = "Your OTP",
                Body =  IsReset
                        ?EmailBodyHelper.GetResetPasswordEmailBody(OTP)
                        : EmailBodyHelper.GetVerificationEmailBody(OTP)    // $"Your OTP is {OTP}"
            };
            await _EmailService.SendEmail(emailrequest);
            return AuthModel.Succeed("تم إرسال رمز التحقق إلى بريدك الإلكتروني", account.Email);
        }
        public async Task<AuthModel> VerifyOtpAsync(string Email, string enteredOtp)
        {          
            Account? Account = await _AccountManager.FindByEmailAsync(Email);
            if (Account == null)
            {
               return AuthModel.Error("المستخدم غير موجود");
            }
            if (Account.EmailConfirmed)
            {
                return AuthModel.Error("email is already confirmed", Account.EmailConfirmed);
            }
            var key = $"OTP_{Email}";
            var cachedOTP = _cache.GetData<string>(key);
           
            if (cachedOTP == null || cachedOTP != enteredOtp)
            {
                return AuthModel.Error("رمز التحقق غير صالح أو منتهي الصلاحية", Account.EmailConfirmed);
            }
       
            _cache.Remove(key);
            Account.EmailConfirmed = true;
            await _AccountManager.UpdateAsync(Account);

            var JwtSecurityToken = await CreateJwtToken(Account);
           // return AuthModel.Succeed("authed", Account.Id, Account.Email, true, new JwtSecurityTokenHandler().WriteToken(JwtSecurityToken), await _AccountManager.GetRolesAsync(Account), JwtSecurityToken.ValidTo);
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
        }
        private string GenerateOTP()
        {
            Random random = new Random();
            string randomNumber = random.Next(0, 1000000).ToString("D6");
            return randomNumber;
        }
        #endregion


        #region password
        public async Task<AuthModel> ForgotPasswordAsync(string Email)
        {
           
            var Account = await _AccountManager.FindByEmailAsync(Email);
            if (Account == null)
            {
                return AuthModel.Error("المستخدم غير موجود");
            }

            return await SendOTP(Email,true);
        }
        public async Task<AuthModel> ChangePasswordAsync(ChangePasswordDTO DTO)
        {
            var Account = await _AccountManager.FindByEmailAsync(DTO.Email);
            if (Account == null)
            {
                return AuthModel.Error("المستخدم غير موجود");
            }
            var checkpassword = await _AccountManager.CheckPasswordAsync(Account, DTO.OldPassword);
            if (!checkpassword)
            {
                return AuthModel.Error("كلمة المرور غير صحيحة ");
            }
            var result = await _AccountManager.ChangePasswordAsync(Account, DTO.OldPassword, DTO.newPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(" , ", result.Errors.Select(e => e.Description));
                return AuthModel.Error(errors);
            }

            return AuthModel.Succeed("تم تغيير كلمة المرور بنجاح");
        }
    
        public async Task<AuthModel> ResetPasswordAsync(ResetPasswordDTO DTO)
        {
            
            var Account = await _AccountManager.FindByEmailAsync(DTO.email);
            if (Account == null)
            {
                return AuthModel.Error("المستخدم غير موجود");
            }
             var key = $"OTP_{DTO.email}";
            var cachedOTP = _cache.GetData<string>(key);
            if (DTO.OTP==null||cachedOTP == null || cachedOTP != DTO.OTP)
            {
                return AuthModel.Error("رمز التحقق غير صالح أو منتهي الصلاحية", Account.EmailConfirmed);
            }

            //if (DTO.OTP == null || DTO.OTP != Account.OTP || DateTime.UtcNow > Account.OTPExpiry)
            //{
            //    return AuthModel.Error("رمز التحقق غير صحيح أو منتهي الصلاحية");
            //}
            var passwordValidators = _AccountManager.PasswordValidators;
            foreach (var validator in passwordValidators)
            {
                var validationResult = await validator.ValidateAsync(_AccountManager, Account, DTO.newPasswod);
                if (!validationResult.Succeeded)
                {
                    var errors = string.Join(" , ", validationResult.Errors.Select(e => e.Description));
                    return AuthModel.Error(errors);
                }
            }
                string NewHashedPassword = _AccountManager.PasswordHasher.HashPassword(Account, DTO.newPasswod);
            Account.PasswordHash = NewHashedPassword;

            //Account.OTP = null;
            //Account.OTPExpiry = null;
            _cache.Remove(key);
            var result = await _AccountManager.UpdateAsync(Account);
            if (!result.Succeeded)
            {
                var errors = string.Join(" , ", result.Errors.Select(e => e.Description));
                return AuthModel.Error(errors);
            }
            var JwtSecurityToken = await CreateJwtToken(Account);
            //  return AuthModel.Succeed("authed", Account.Id, Account.Email, true, new JwtSecurityTokenHandler().WriteToken(JwtSecurityToken), await _AccountManager.GetRolesAsync(Account), JwtSecurityToken.ValidTo);
            return new AuthModel
            {
                Success = true,
                StatusCode = StatusCode.Ok,
                message = "تم تغيير كلمة المرور بنجاح",
                Id = Account.Id,
                Email = Account.Email,
                IsEmailConfirmed = Account.EmailConfirmed,
                ExpiresOn = JwtSecurityToken.ValidTo,
                isAuthenticated = true,
                Roles = await _AccountManager.GetRolesAsync(Account),
                Token = new JwtSecurityTokenHandler().WriteToken(JwtSecurityToken),

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

        public async Task<AuthModel> SignInWithGoogleAsync(GooglesigninDTO model)
        {
            var response = await _googleAuthService.GoogleSignIn(model);

            if (!response.Success)
            {
                return AuthModel.Error(response.message);
            }

            if (response.Data == null)
            {
                return AuthModel.Error("Failed to retrieve user data.");
            }
            var jwtSecurityToken = await CreateJwtToken(response.Data);
            var user = response.Data;
            //   return AuthModel.Succeed("authed", user.Id, user.Email, true, new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken), await _AccountManager.GetRolesAsync(user), jwtSecurityToken.ValidTo);
            return new AuthModel
            {
                Success=true,
                StatusCode = StatusCode.Ok,
                Id = user.Id,
                IsEmailConfirmed = true,
                isAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Email = user.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                Roles = await _AccountManager.GetRolesAsync(user),
            };
        }
        #endregion



    }
}
