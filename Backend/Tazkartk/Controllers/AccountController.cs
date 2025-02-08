using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tazkartk.DTO;
using Tazkartk.Interfaces;

namespace Tazkartk.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("Forget-Password")]
        public async Task<IActionResult> ForgetPassword(SendOTPDTO DTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.ForgetPassword(DTO.Email);
            return Ok(result);

        }
        [HttpPost("Reset-Password")]

        public async Task<IActionResult> ResetPassword(ResetPasswordDTO DTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.ResetPassword(DTO);
            return Ok(result);
        }
        [Authorize]
        [HttpPost("Change-Password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO DTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.ChangePassword(DTO);
            return Ok(result);
        }
        [HttpPost("Admin-Register")]
        public async Task<IActionResult> AdminRegister([FromBody] RegisterDTO userRegisterDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.RegisterAsync(userRegisterDTO);
            if (!result.isAuthenticated) return BadRequest(result.message);

            //SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);

        }
        [HttpPost("Send-OTP")]
        public async Task<IActionResult> SendOtp([FromBody] SendOTPDTO DTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.SendOTP(DTO.Email);
            return Ok(result);

        }
        [HttpPost("Verify-OTP")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOTPDTO verifyOtpDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.VerifyOtpAsync(verifyOtpDTO.Email, verifyOtpDTO.EnteredOtp);
            if (!result.isAuthenticated) return BadRequest(result.message);

            return Ok(result);
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO userRegisterDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.RegisterAsync(userRegisterDTO);
            if(!result.Success) return BadRequest(result.message);
            
            return Ok(result);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.LoginAsync(loginDTO);
            if (!result.isAuthenticated) return BadRequest(result.message);
            return Ok(result);
        }
        [HttpPost("Company-Register")]
        public async Task<IActionResult> RegisterCompany([FromBody] CompanyRegisterDTO CompanyRegisterDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.CompanyRegisterAsync(CompanyRegisterDTO);
            if (!result.isAuthenticated) return BadRequest(result.message);

            //SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);

        }
        [HttpPost("Company-Login")]
        public async Task<IActionResult> LoginCompany([FromBody] LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.CompanyLoginAsync(loginDTO);
            if (!result.isAuthenticated) return BadRequest(result.message);
            //if(!string.IsNullOrEmpty(result.RefreshToken))
            //{
            //    SetRefreshTokenInCookie(result.RefreshToken,result.RefreshTokenExpiration);
            //}
            return Ok(result);

        }

    }
}