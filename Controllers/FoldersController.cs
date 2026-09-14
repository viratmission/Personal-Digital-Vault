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
    public class FoldersController : ControllerBase
    {
        private readonly IFolderService _folderService;

        public FoldersController(IFolderService folderService)
        {
            _folderService = folderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateFolder(CreateFolderDto createFolderDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);

            var folder = await _folderService.CreateFolderAsync(
                createFolderDto,
                userId);

            return Ok(folder);
        }

        [HttpGet]
        public async Task<IActionResult> GetFolders()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);

            var folders = await _folderService.GetFoldersByUserIdAsync(userId);

            return Ok(folders);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> RenameFolder(
           int id,
           RenameFolderDto renameFolderDto)

        {
             var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);

            var folder = await _folderService.RenameFolderAsync(
                id,
                renameFolderDto,
                userId);

            if (folder == null)
            {
                return NotFound();
            }

            return Ok(folder);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFolder(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);
            var deleted = await _folderService.DeleteFolderAsync(id, userId);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();

            
        }
    }
}