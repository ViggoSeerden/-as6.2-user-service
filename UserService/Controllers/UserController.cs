using Microsoft.AspNetCore.Mvc;
using UserServiceBusiness.Models;
using UserServiceBusiness.Services;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController(UserServiceBusiness.Services.UserService userService) : ControllerBase
    {
        [HttpGet("")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await userService.GetAllUsersAsync();
            return Ok(users);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await userService.GetUserByIdAsync(id);
            return Ok(user);
        }
        
        [HttpPost("")]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await userService.AddUserAsync(user);
            return NoContent();
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] User updatedUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            await userService.UpdateUserAsync(updatedUser);
            return NoContent();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            await userService.DeleteUserAysnc(id);
            return Ok();
        }
    }
}