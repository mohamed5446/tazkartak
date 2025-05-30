using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Tazkartk.Application;
using Tazkartk.Application.DTO.AccontDTOs;
using Tazkartk.Application.DTO.UserDTOs;
using Tazkartk.Application.Interfaces;

namespace Tazkartk.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;

        }
        [HttpGet("Admins")]
        //  [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "List All Admins")]
        public async Task<IActionResult> GetAdmins()
        {
            var users = await _userService.GetUsersAsync(Roles.Admin);
            return users == null ? NotFound() : Ok(users);
        }

        [HttpGet]
        //  [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "List All Users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetUsersAsync(Roles.User);
            return users == null ? NotFound() : Ok(users);
        }


        [HttpGet("{Id:int}")]
        //  [Authorize(Roles = "Admin , Company")]
        [SwaggerOperation(Summary = "Get User By Id")]
        public async Task<IActionResult> GetUser(int Id)
        {
            var user = await _userService.GetUserByIdAsync(Id);
            return user == null ? NotFound("User Not Found") : Ok(user);
        }
        [HttpPost]
        //  [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Add User")]

        public async Task<IActionResult> CreateUser(RegisterDTO DTO)
        {
            var result = await _userService.AddUserAsync(DTO, Roles.User);
            return StatusCode((int)result.StatusCode, result);
        }
        [HttpPost("Add-Admin")]
        // [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Add Admin")]

        public async Task<IActionResult> AddAdmin(RegisterDTO DTO)
        {
            var result = await _userService.AddUserAsync(DTO, Roles.Admin);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPut("{Id:int}")]
        // [Authorize(Roles = "Admin , User")]
        [SwaggerOperation(Summary = "Edit User")]

        //[Authorize]
        public async Task<IActionResult> EditUser(int Id, [FromForm] EditUserDTO DTO)
        {
            var result = await _userService.EditUserAsync(Id, DTO);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpDelete("{Id:int}")]
        // [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Delete User")]

        public async Task<IActionResult> DeleteUser(int Id)
        {
            var result = await _userService.DeleteUserAsync(Id);
            return StatusCode((int)result.StatusCode, result);
        }

    }
}
