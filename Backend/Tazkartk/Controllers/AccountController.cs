using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tazkartk.DTO;
using Tazkartk.Interfaces;
using Tazkartk.Models.Enums;

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
        [HttpPost("Register")]
        public async Task<IActionResult> Register( RegisterDTO userRegisterDTO)
        {
            var result = await _authService.RegisterAsync(userRegisterDTO);
            if (!result.Success) return BadRequest(result.message);
            return Ok(result);
        }

        [HttpPost("Company-Register")]
        public async Task<IActionResult> RegisterCompany(CompanyRegisterDTO CompanyRegisterDTO)
        {
            var result = await _authService.CompanyRegisterAsync(CompanyRegisterDTO);
            if (!result.isAuthenticated) return BadRequest(result.message);
            return Ok(result);
        }

        [HttpPost("Admin-Register")]
        public async Task<IActionResult> AdminRegister(RegisterDTO userRegisterDTO)
        {
            var result = await _authService.RegisterAsync(userRegisterDTO, Roles.Admin);
            if (!result.Success) return BadRequest(result.message);
            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var result = await _authService.LoginAsync(loginDTO);
            if (!result.isAuthenticated) return BadRequest(result.message);
            return Ok(result);
        }

        [HttpPost("Send-OTP")]
        public async Task<IActionResult> SendOtp(SendOTPDTO DTO)
        {
            var result = await _authService.SendOTP(DTO.Email);
            return Ok(result);
        }
        [HttpPost("Verify-OTP")]
        public async Task<IActionResult> VerifyOtp(VerifyOTPDTO verifyOtpDTO)
        {
            var result = await _authService.VerifyOtpAsync(verifyOtpDTO.Email, verifyOtpDTO.EnteredOtp);
            if (!result.isAuthenticated) return BadRequest(result.message);
            return Ok(result);
        }


        [HttpPost("Forget-Password")]
        public async Task<IActionResult> ForgetPassword(SendOTPDTO DTO)
        {          
            var result = await _authService.ForgetPassword(DTO.Email);
            return Ok(result);
        }
        [HttpPost("Reset-Password")]

        public async Task<IActionResult> ResetPassword(ResetPasswordDTO DTO)
        {         
            var result = await _authService.ResetPassword(DTO);
            return Ok(result);
        }
        [Authorize]
        [HttpPost("Change-Password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO DTO)
        {          
            var result = await _authService.ChangePassword(DTO);
            return Ok(result);
        }
      
      
      
       
       
       
    }
}