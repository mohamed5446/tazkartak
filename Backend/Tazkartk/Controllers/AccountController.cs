using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tazkartk.DTO;
using Tazkartk.Services;

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
        public async Task<IActionResult> Register([FromBody] RegisterDTO userRegisterDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.RegisterAsync(userRegisterDTO);
            if (!result.isAuthenticated) return BadRequest(result.message);
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
    }
}