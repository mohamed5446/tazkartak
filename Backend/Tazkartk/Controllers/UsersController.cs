using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Tazkartk.DTO;
using Tazkartk.DTO.AccontDTOs;
using Tazkartk.DTO.Response;
using Tazkartk.DTO.UserDTOs;
using Tazkartk.Interfaces;
using Tazkartk.Models.Enums;

namespace Tazkartk.Controllers
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
        
        [HttpGet]
      //  [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "List All Users")]
        public async Task<IActionResult>GetUsers()
        {
            var users = await _userService.GetUsers(); 
            return users==null?NotFound():Ok(users);
        }


        [HttpGet("{Id:int}")]
      //  [Authorize(Roles = "Admin , Company")]
        [SwaggerOperation(Summary = "Get User By Id")]
        public async Task<IActionResult>GetUser(int Id)
        {
            var user = await _userService.GetUserById(Id);
            return user == null ? NotFound("User Not Found"):Ok(user);
        }
        [HttpPost]
      //  [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Add User")]

        public async Task<IActionResult>CreateUser(RegisterDTO DTO)
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
            var result = await _userService.AddUser(DTO,Roles.User);
            return StatusCode((int)result.StatusCode,result);   
        }
        [HttpPost("Add-Admin")]
       // [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Add Admin")]

        public async Task<IActionResult> AddAdmin(RegisterDTO DTO)
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
            var result = await _userService.AddUser(DTO, Roles.Admin);
            return StatusCode((int)result.StatusCode,result);
        }

        [HttpPut("{Id:int}")]
       // [Authorize(Roles = "Admin , User")]
        [SwaggerOperation(Summary = "Edit User")]

        //[Authorize]
        public async Task<IActionResult>EditUser(int Id ,[FromForm]EditUserDTO DTO)
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
            var result = await _userService.EditUser(Id, DTO);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpDelete("{Id:int}")]
       // [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Delete User")]

        public async Task<IActionResult> DeleteUser(int Id)
        {
           var result=  await _userService.DeleteUser(Id);
            return StatusCode((int)result.StatusCode,result);
        }
       
    }
}
