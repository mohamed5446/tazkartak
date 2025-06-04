using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Tazkartk.Application.DTO.AccontDTOs;
using Tazkartk.Application.DTO.CompanyDTOs;
using Tazkartk.Application.DTO.Google;
using Tazkartk.Application.Interfaces;

namespace Tazkartk.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AccountController(IAuthService authService )
        {
            _authService = authService;
        }
        //[Authorize]
        //[HttpGet("curr")]
        //public async Task<IActionResult>Getcurrent()
        //{
        //    var id = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        //    //var email = User.FindFirst(ClaimTypes.Email);
        //    return Ok(id);
        //}
       
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO userRegisterDTO)
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
            var result = await _authService.RegisterAsync(userRegisterDTO, Application.Roles.Admin);
            return StatusCode((int)result.StatusCode, result);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var result = await _authService.LoginAsync(loginDTO);
            return StatusCode((int)result.StatusCode, result);
        }
        [HttpPost("SignIn-Google")]
        [SwaggerOperation(Summary = "Sign Up/In With Google")]
        public async Task<IActionResult> GoogleSignIn(GooglesigninDTO model)
        {
            var result = await _authService.SignInWithGoogleAsync(model);
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
       // [Authorize]
        [HttpPut("Change-Password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO DTO)
        {
            var result = await _authService.ChangePasswordAsync(DTO);
            return StatusCode((int)result.StatusCode, result);
        }

    }
}