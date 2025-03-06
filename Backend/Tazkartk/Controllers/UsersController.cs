using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tazkartk.DTO.UserDTOs;
using Tazkartk.Interfaces;

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
        public async Task<IActionResult>GetUsers()
        {
            var users = await _userService.GetUsers(); 
            return users==null?NotFound():Ok(users);
        }


        [HttpGet("{id:int}")]
        public async Task<IActionResult>GetUser(int id)
        {
            var user = await _userService.GetUserDetailsById(id);
            return user==null? NotFound("user not found ") : Ok(user);   
        }


        [HttpPut("{id:int}")]
        //[Authorize]
        public async Task<IActionResult>EditUser(int id ,[FromForm]EditUserDTO DTO)
        {
            
            var user=await _userService.GetUserById(id);
            if(user == null)
            {
                return NotFound();
            }
          var updated =await _userService.EditUser(user, DTO);
            
            return Ok(new { message = "edit succeed", User = updated });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user =await _userService.GetUserById(id);
            if (user == null) return NotFound();
             await _userService.DeleteUser(user);
            return NoContent();
        }
       
    }
}
