using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalDigitalVault.DTOs;
using PersonalDigitalVault.Interfaces;
using System.Security.Claims;

namespace PersonalDigitalVault.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CredentialController : ControllerBase
    {
        private readonly ICredentialService _credentialService;

        public CredentialController(
            ICredentialService credentialService)
        {
            _credentialService = credentialService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            CreateCredentialDto dto)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            var result =
                await _credentialService.CreateAsync(
                    userId.Value,
                    dto);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            var result =
                await _credentialService.GetAllAsync(
                    userId.Value);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            var result =
                await _credentialService.GetByIdAsync(
                    id,
                    userId.Value);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateCredentialDto dto)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            var result =
                await _credentialService.UpdateAsync(
                    id,
                    userId.Value,
                    dto);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            var deleted =
                await _credentialService.DeleteAsync(
                    id,
                    userId.Value);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return null;
            }

            if (!int.TryParse(
                    userIdClaim.Value,
                    out var userId))
            {
                return null;
            }

            return userId;
        }
    }
}