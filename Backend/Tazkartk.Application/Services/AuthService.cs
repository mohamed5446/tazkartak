using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Tazkartk.Domain.Models;
using AutoMapper;
using Tazkartk.Application.DTO.AccontDTOs;
using Tazkartk.Application.DTO.CompanyDTOs;
using Tazkartk.Application.DTO.Response;
using Tazkartk.Application.Interfaces;
using Tazkartk.Application.DTO.Google;
using Tazkartk.Application.DTO.Email;
using Tazkartk.Application.Interfaces.External;
using System.Text;
using System.Security.Cryptography;

namespace Tazkartk.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<Account> _AccountManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _conf;
        private readonly IEmailService _EmailService;
        private readonly IGoogleAuthService _googleAuthService;
        private readonly ICachingService _cache;
        private readonly IEmailBodyService _emailBodyService;
        private readonly ITokenService _tokenService;

        public AuthService(IConfiguration conf, UserManager<Account> AccountManager, IEmailService emailService, IGoogleAuthService googleAuthService, IMapper mapper, ICachingService cachingService, ITokenService tokenService, IEmailBodyService emailBodyService)
        {
            _AccountManager = AccountManager;
            _EmailService = emailService;
            _conf = conf;
            _googleAuthService = googleAuthService;
            _mapper = mapper;
            _cache = cachingService;
            _tokenService = tokenService;
            _emailBodyService = emailBodyService;
        }
        #region User
        public async Task<AuthModel> RegisterAsync(RegisterDTO DTO, Roles Role = Roles.User)
        {

            if (await _AccountManager.FindByEmailAsync(DTO.Email) != null)
            {
                return AuthModel.Error("البريد الإلكتروني مستخدم من قبل");
            }

            var user = _mapper.Map<User>(DTO);
            user.photo = _conf["Avatar"];
            user.UserName = user.Email;

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
        
            var token = await _tokenService.GenerateToken(Account);
            return new AuthModel
            {
                Success = true,
                StatusCode = StatusCode.Ok,
                Id = Account.Id,
                IsEmailConfirmed = Account.EmailConfirmed,
                isAuthenticated = true,
                Token = token,
                Email = Account.Email,
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
            var Company = _mapper.Map<Company>(DTO);
            Company.Logo = _conf["Logo"];
            Company.UserName = Company.Email;

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
        public async Task<AuthModel> SendOTP(string email, bool IsReset = false)
        {    
            var account = await _AccountManager.FindByEmailAsync(email);
            if (account == null)
            {
                return AuthModel.Error("لم يتم العثور على حساب ");
            }
            var key = $"OTP_{email}";
            var existingOtp = _cache.GetData<string>(key);
            if (existingOtp != null)
            {
                return AuthModel.Error("يوجد رمز تحقق تم إرساله بالفعل");
            }
            
            string OTP = GenerateOTP();
            var hashedotp = ComputeHash(OTP);
            _cache.SetData(key, hashedotp, 5);
          
            await _AccountManager.UpdateAsync(account);
            var emailrequest = new EmailRequest
            {
                Email = account.Email,
                Subject = "Your OTP",
                Body = IsReset
                          ?_emailBodyService.ResetPasswordEmailBody(OTP)
                          :_emailBodyService.ConfirmationEmailBody(OTP)
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
            var hashedenteredOTP= ComputeHash(enteredOtp);
            if (cachedOTP == null || cachedOTP != hashedenteredOTP)
            {
                return AuthModel.Error("رمز التحقق غير صالح أو منتهي الصلاحية", Account.EmailConfirmed);
            }

            _cache.Remove(key);
            Account.EmailConfirmed = true; 
            await _AccountManager.UpdateAsync(Account);

            var token = await _tokenService.GenerateToken(Account);

            return new AuthModel
            {
                Success = true,
                StatusCode = StatusCode.Ok,
                Id = Account.Id,
                IsEmailConfirmed = Account.EmailConfirmed,
                Email = Account.Email,
                isAuthenticated = true,
                Roles = await _AccountManager.GetRolesAsync(Account),
                Token = token
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
            return await SendOTP(Email, true);
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
            if (DTO.OTP == null || cachedOTP == null || cachedOTP != DTO.OTP)
            {
                return AuthModel.Error("رمز التحقق غير صالح أو منتهي الصلاحية", Account.EmailConfirmed);
            }

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

            _cache.Remove(key);
            var result = await _AccountManager.UpdateAsync(Account);
            if (!result.Succeeded)
            {
                var errors = string.Join(" , ", result.Errors.Select(e => e.Description));
                return AuthModel.Error(errors);
            }
            var token = await _tokenService.GenerateToken(Account);
            return new AuthModel
            {
                Success = true,
                StatusCode = StatusCode.Ok,
                message = "تم تغيير كلمة المرور بنجاح",
                Id = Account.Id,
                Email = Account.Email,
                IsEmailConfirmed = Account.EmailConfirmed,
                isAuthenticated = true,
                Roles = await _AccountManager.GetRolesAsync(Account),
                Token = token,

            };
        }
        #endregion

        public async Task<AuthModel> SignInWithGoogleAsync(GooglesigninDTO model)
        {
            var response = await _googleAuthService.GoogleSignIn(model);
            if(response == null)
            {
                 return AuthModel.Error("Failed to retrieve user data.");
            }
            var user = response;
            var token=await _tokenService.GenerateToken(user);
            return new AuthModel
            {
                Success = true,
                StatusCode = StatusCode.Ok,
                Id = user.Id,
                IsEmailConfirmed = true,
                isAuthenticated = true,
                Token = token,
                Email = user.Email,
                Roles = await _AccountManager.GetRolesAsync(user),
                PhoneNumberNeeded=true,
            };
        }
        private string ComputeHash(string data)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                byte[] hash = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
