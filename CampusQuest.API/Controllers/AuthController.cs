using CampusQuest.Domain.Entities;
using CampusQuest.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace CampusQuest.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;

        public AuthController(UserService userService)
        {
            _userService = userService;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var user = await _userService.RegisterAsync(request.Username, request.Email, request.Password);

            if (user == null)
            {
                return BadRequest(new { message = "Username or Email is already taken." });
            }

            return Ok(new { message = "Registration successful!", userId = user.Id });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userService.LoginAsync(request.Email, request.Password);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            // For now, we return user info. Later we can integrate JWT Token.
            return Ok(new
            {
                message = "Login successful!",
                user = new { user.Id, user.Username, user.Email, user.Level, user.Xp }
            });
        }
    }

    // DTOs (Data Transfer Objects) for handling requests safely
    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}