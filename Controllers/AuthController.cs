using Microsoft.AspNetCore.Mvc;
using PersonalDigitalVault.DTOs;
using PersonalDigitalVault.Interfaces;

namespace PersonalDigitalVault.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            bool result = await _authService.RegisterAsync(registerDto);

            if (!result)
            {
                return BadRequest("Email already exists.");
            }

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var loginResponse = await _authService.LoginAsync(loginDto);

            if (loginResponse == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            return Ok(loginResponse);
        }
    }
}