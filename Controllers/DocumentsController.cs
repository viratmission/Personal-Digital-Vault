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
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentsController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadDocument(
            [FromForm] UploadDocumentDto uploadDocumentDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);

            try
            {
                var document = await _documentService.UploadDocumentAsync(
                    uploadDocumentDto,
                    userId);

                if (document == null)
                {
                    return NotFound(
                        "Folder not found or does not belong to the current user.");
                }

                return Ok(document);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetDocuments()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);

            var documents = await _documentService
                .GetDocumentsByUserIdAsync(userId);

            return Ok(documents);
        }
        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadDocument(int id)
        {
            var userIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(
                userIdClaim.Value);

            try
            {
                var result = await _documentService
                    .GetDocumentForDownloadAsync(
                        id,
                        userId);

                if (result.Document == null)
                {
                    return NotFound();
                }

                if (result.FileBytes == null)
                {
                    return NotFound(
                        "Physical file not found.");
                }

                return File(
                    result.FileBytes,
                    result.Document.ContentType,
                    result.Document.FileName);
            }
            catch (InvalidDataException ex)
            {
                return Conflict(ex.Message);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> RenameDocument(
            int id,
            RenameDocumentDto renameDocumentDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);

            var document = await _documentService.RenameDocumentAsync(
                id,
                renameDocumentDto,
                userId);

            if (document == null)
            {
                return NotFound();
            }

            return Ok(document);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);

            var deleted = await _documentService.DeleteDocumentAsync(
                id,
                userId);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}