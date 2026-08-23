using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalDigitalVault.DTOs;
using PersonalDigitalVault.Interfaces;
using System.Security.Claims;

namespace PersonalDigitalVault.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IAuthService _authService;

        public ProfileController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);

            var profile = await _authService.GetProfileAsync(userId);

            if (profile == null)
            {
                return NotFound("User not found.");
            }

            return Ok(profile);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto updateProfileDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);

            bool result =
                await _authService.UpdateProfileAsync(
                    userId,
                    updateProfileDto
                );

            if (!result)
            {
                return BadRequest("Profile update failed.");
            }

            return Ok("Profile updated successfully.");
        }
    }
}