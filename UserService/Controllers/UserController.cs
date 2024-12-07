using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserServiceBusiness.Models;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController(UserServiceBusiness.Services.UserService userService) : ControllerBase
    {
        [HttpGet("")]
        [Authorize("read:users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await userService.GetAllUsersAsync();
            return Ok(users);
        }
        
        [HttpGet("{id}")]
        [Authorize("read:user")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await userService.GetUserByIdAsync(id);
            return Ok(user);
        }
        
        [HttpGet("self")]
        [Authorize("read:self")]
        public async Task<IActionResult> GetSelf(Guid id)
        {
            var user = await userService.GetUserByIdAsync(id);
            return Ok(user);
        }        
        
        [HttpGet("self/email")]
        [Authorize("read:self")]
        public async Task<IActionResult> GetSelfByEmail(string email)
        {
            var user = await userService.GetUserByEmail(email);
            return Ok(user);
        }
        
        [HttpPost("")]
        [Authorize("write:add_user")]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await userService.AddUserAsync(user);
            return NoContent();
        }
        
        [HttpPut("{id}")]
        [Authorize("write:update_user")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] User updatedUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            await userService.UpdateUserAsync(updatedUser);
            return NoContent();
        }
        
        [HttpDelete("{id}")]
        [Authorize("write:delete_user")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            await userService.DeleteUserAsync(id);
            return Ok();
        }
    }
}