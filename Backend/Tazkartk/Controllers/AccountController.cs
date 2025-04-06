using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tazkartk.DTO;
using Tazkartk.DTO.AccontDTOs;
using Tazkartk.DTO.CompanyDTOs;
using Tazkartk.DTO.Response;
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
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorMessage = string.Join("; ", errorMessages);

                return StatusCode(400, new ApiResponse<string>
                {
                    Success = false,
                    StatusCode = Models.Enums.StatusCode.BadRequest,
                    message = errorMessage
                });
            }
            var result = await _authService.RegisterAsync(userRegisterDTO);
                return StatusCode((int)result.StatusCode, result);
            }

            [HttpPost("Company-Register")]
            public async Task<IActionResult> RegisterCompany(CompanyRegisterDTO CompanyRegisterDTO)
            {
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorMessage = string.Join("; ", errorMessages);

                return StatusCode(400, new ApiResponse<string>
                {
                    Success = false,
                    StatusCode = Models.Enums.StatusCode.BadRequest,
                    message = errorMessage
                });
            }
            var result = await _authService.CompanyRegisterAsync(CompanyRegisterDTO);
               return StatusCode((int)result.StatusCode, result);
            }

            [HttpPost("Admin-Register")]
            public async Task<IActionResult> AdminRegister(RegisterDTO userRegisterDTO)
            {
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorMessage = string.Join("; ", errorMessages);

                return StatusCode(400, new ApiResponse<string>
                {
                    Success = false,
                    StatusCode = Models.Enums.StatusCode.BadRequest,
                    message = errorMessage
                });
            }
            var result = await _authService.RegisterAsync(userRegisterDTO, Roles.Admin);
                return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("Login")]
            public async Task<IActionResult> Login(LoginDTO loginDTO)
            {
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorMessage = string.Join("; ", errorMessages);

                return StatusCode(400, new ApiResponse<string>
                {
                    Success = false,
                    StatusCode = Models.Enums.StatusCode.BadRequest,
                    message = errorMessage
                });
            }
            var result = await _authService.LoginAsync(loginDTO);
               return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("Send-OTP")]
            public async Task<IActionResult> SendOtp(SendOTPDTO DTO)
            {
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorMessage = string.Join("; ", errorMessages);

                return StatusCode(400, new ApiResponse<string>
                {
                    Success = false,
                    StatusCode = Models.Enums.StatusCode.BadRequest,
                    message = errorMessage
                });
            }
            var result = await _authService.SendOTP(DTO.Email);
            return StatusCode((int)result.StatusCode, result);
        }
        [HttpPost("Verify-OTP")]
            public async Task<IActionResult> VerifyOtp(VerifyOTPDTO verifyOtpDTO)
            {
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorMessage = string.Join("; ", errorMessages);

                return StatusCode(400, new ApiResponse<string>
                {
                    Success = false,
                    StatusCode = Models.Enums.StatusCode.BadRequest,
                    message = errorMessage
                });
            }
            var result = await _authService.VerifyOtpAsync(verifyOtpDTO.Email, verifyOtpDTO.EnteredOtp);
               return StatusCode((int)result.StatusCode, result);
        }


        [HttpPost("Forget-Password")]
            public async Task<IActionResult> ForgetPassword(SendOTPDTO DTO)
            {
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorMessage = string.Join("; ", errorMessages);

                return StatusCode(400, new ApiResponse<string>
                {
                    Success = false,
                    StatusCode = Models.Enums.StatusCode.BadRequest,
                    message = errorMessage
                });
            }
            var result = await _authService.ForgetPassword(DTO.Email);
                return StatusCode((int)result.StatusCode, result);
        }
        [HttpPut("Reset-Password")]

            public async Task<IActionResult> ResetPassword(ResetPasswordDTO DTO)
            {
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorMessage = string.Join("; ", errorMessages);

                return StatusCode(400, new ApiResponse<string>
                {
                    Success = false,
                    StatusCode = Models.Enums.StatusCode.BadRequest,
                    message = errorMessage
                });
            }
            var result = await _authService.ResetPassword(DTO);
                return StatusCode((int)result.StatusCode, result);
            }
       // [Authorize(Roles = "User")]
        [HttpPut("Change-Password")]
            public async Task<IActionResult> ChangePassword(ChangePasswordDTO DTO)
            {
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorMessage = string.Join("; ", errorMessages);

                return StatusCode(400, new ApiResponse<string>
                {
                    Success = false,
                    StatusCode = Models.Enums.StatusCode.BadRequest,
                    message = errorMessage
                });
            }
            var result = await _authService.ChangePassword(DTO);
                return StatusCode((int)result.StatusCode, result);
            }
      
       
      
       
       
       
        }
    }