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
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet]
        public async Task<IActionResult> Search(
            [FromQuery] string? searchTerm,
            [FromQuery] string? itemType)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim);

            var results = await _searchService.SearchAsync(
                userId,
                searchTerm,
                itemType
            );

            return Ok(results);
        }
    }
}