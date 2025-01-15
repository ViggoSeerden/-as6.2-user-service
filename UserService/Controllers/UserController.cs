using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserServiceBusiness.Models;
using UserServiceBusiness.Services;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly MessageProducer _sender;
        private readonly UserServiceBusiness.Services.UserService _userService;

        public UserController(MessageProducer sender, UserServiceBusiness.Services.UserService userService)
        {
            _sender = sender;
            _userService = userService;
        }
        
        
        [HttpGet("")]
        [Authorize("read:users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize("read:user")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return Ok(user);
        }

        [HttpGet("self")]
        [Authorize("read:self")]
        public async Task<IActionResult> GetSelf(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return Ok(user);
        }

        [HttpGet("self/email")]
        [Authorize("read:self")]
        public async Task<IActionResult> GetSelfByEmail(string email)
        {
            var user = await _userService.GetUserByEmail(email);
            return Ok(user);
        }

        [HttpPost("")]
        [Authorize("write:add_user")]
        public async Task<IActionResult> AddUser([FromBody] UserDto user, [FromHeader] string authorization)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var token = authorization.Substring("Bearer ".Length).Trim();
            var claimsPrincipal = DecodeToken(token);
            var emailClaim = claimsPrincipal.FindFirst("sub")?.Value;

            if (emailClaim == null)
                return BadRequest();
            
            var newUser = new User
            {
                Name = user.Name,
                Email = user.Email,
                ProviderAccountId = emailClaim,
                RoleId = 4
            };

            var users = await _userService.GetAllUsersAsync();

            foreach (var existingUser in users)
            {
                if (existingUser.ProviderAccountId == emailClaim)
                    return StatusCode(208);

                if (existingUser.Email == newUser.Email)
                    return StatusCode(208);
            }
            
            var requestId = Guid.NewGuid();
            var emailBytes = System.Text.Encoding.UTF8.GetBytes(user.Email);
            _sender.SendMessage($"email.request.{requestId}.{Convert.ToBase64String(emailBytes)}", $"Welcome to Osso Online, {user.Name}");

            await _userService.AddUserAsync(newUser);
            return Created();
        }

        [HttpPut("{id}")]
        [Authorize("write:update_user")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] User updatedUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userService.GetUserByIdAsync(id);
            if (user != null)
                return NotFound();

            _userService.UpdateUser(updatedUser);
            return Created();
        }

        [HttpDelete("{id}")]
        [Authorize("write:delete_user")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }
        
        private ClaimsPrincipal DecodeToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var expirationTime = jwtToken.ValidTo;

            if (expirationTime < DateTime.UtcNow)
            {
                throw new Exception("Token has expired.");
            }

            return new ClaimsPrincipal(new ClaimsIdentity(jwtToken.Claims));
        }
    }
}