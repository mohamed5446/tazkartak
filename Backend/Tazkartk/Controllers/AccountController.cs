using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Tazkartk.DTO;
using Tazkartk.DTO.AccontDTOs;
using Tazkartk.DTO.CompanyDTOs;
using Tazkartk.DTO.Response;
using Tazkartk.Google;
using Tazkartk.Interfaces;
using Tazkartk.Models.Enums;
using Tazkartk.SMS;

namespace Tazkartk.Controllers
{
        [Route("api/[controller]")]
        [ApiController]
        public class AccountController : ControllerBase
        {
        private readonly IAuthService _authService;   
        private readonly ISMSService _smsService;
        public AccountController(IAuthService authService, ISMSService smsService)
        {
            _authService = authService;
            _smsService = smsService;
        }
        //[HttpPost("Send_SMS")]
        //public async Task<IActionResult>SendSMS(SmsDTO DTO)
        //{
        //    var result =await  _smsService.Send(DTO.PhoneNumber, DTO.Body);
        //    if (!string.IsNullOrEmpty(result.ErrorMessage)) return BadRequest(result.ErrorMessage);
        //    return Ok(result);
        //}
        [HttpPost("SignIn-Google")]
        [SwaggerOperation(Summary = "Sign Up/In With Google")]
        public async Task<IActionResult> GoogleSignIn(GooglesigninDTO model)
        {
            var result = await _authService.SignInWithGoogleAsync(model);
            return StatusCode((int)result.StatusCode, result);
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register( RegisterDTO userRegisterDTO)
        {    
        var result = await _authService.RegisterAsync(userRegisterDTO);    
            return StatusCode((int)result.StatusCode, result);    
        }    
        [HttpPost("Company-Register")]
        public async Task<IActionResult> RegisterCompany(CompanyRegisterDTO CompanyRegisterDTO)    
        {               
        var result = await _authService.CompanyRegisterAsync(CompanyRegisterDTO);    
           return StatusCode((int)result.StatusCode, result);    
        }    
        [HttpPost("Admin-Register")]    
        public async Task<IActionResult> AdminRegister(RegisterDTO userRegisterDTO)    
        {    
        var result = await _authService.RegisterAsync(userRegisterDTO, Roles.Admin);    
           return StatusCode((int)result.StatusCode, result);    
        }    
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)    
        {    
        var result = await _authService.LoginAsync(loginDTO);    
           return StatusCode((int)result.StatusCode, result);    
        }    
        [HttpPost("Send-OTP")]
        public async Task<IActionResult> SendOtp(SendOTPDTO DTO)    
        {    
        var result = await _authService.SendOTP(DTO.Email);    
        return StatusCode((int)result.StatusCode, result);    
        }    
        [HttpPost("Verify-OTP")]
        public async Task<IActionResult> VerifyOtp(VerifyOTPDTO verifyOtpDTO)    
        {    
        var result = await _authService.VerifyOtpAsync(verifyOtpDTO.Email, verifyOtpDTO.EnteredOtp);    
           return StatusCode((int)result.StatusCode, result);    
        }    
        [HttpPost("Forget-Password")]
        public async Task<IActionResult> ForgotPassword(SendOTPDTO DTO)
        {
        var result = await _authService.ForgotPasswordAsync(DTO.Email);    
            return StatusCode((int)result.StatusCode, result);    
        }            
        [HttpPut("Reset-Password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO DTO)    
        {            
        var result = await _authService.ResetPasswordAsync(DTO);    
            return StatusCode((int)result.StatusCode, result);    
        }    
        [HttpPut("Change-Password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO DTO)    
        {    
        var result = await _authService.ChangePasswordAsync(DTO);    
            return StatusCode((int)result.StatusCode, result);    
        }    
        }
    }