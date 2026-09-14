using Microsoft.EntityFrameworkCore;
using PersonalDigitalVault.Data;
using PersonalDigitalVault.DTOs;
using PersonalDigitalVault.Interfaces;

namespace PersonalDigitalVault.Services
{
    public class SearchService : ISearchService
    {
        private readonly AppDbContext _context;

        public SearchService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SearchResultDto>> SearchAsync(
            int userId,
            string? searchTerm,
            string? itemType)
        {
            var results = new List<SearchResultDto>();

            searchTerm = searchTerm?.Trim().ToLower();
            itemType = itemType?.Trim().ToLower();

            if (string.IsNullOrEmpty(itemType) || itemType == "folder")
            {
                var folders = await _context.Folders
                    .Where(f =>
                        f.UserId == userId &&
                        (string.IsNullOrEmpty(searchTerm) ||
                         f.Name.ToLower().Contains(searchTerm)))
                    .Select(f => new SearchResultDto
                    {
                        Id = f.Id,
                        ItemType = "Folder",
                        Name = f.Name
                    })
                    .ToListAsync();

                results.AddRange(folders);
            }

            if (string.IsNullOrEmpty(itemType) || itemType == "document")
            {
                var documents = await _context.Documents
                    .Where(d =>
                        d.UserId == userId &&
                        (string.IsNullOrEmpty(searchTerm) ||
                         d.FileName.ToLower().Contains(searchTerm)))
                    .Select(d => new SearchResultDto
                    {
                        Id = d.Id,
                        ItemType = "Document",
                        Name = d.FileName
                    })
                    .ToListAsync();

                results.AddRange(documents);
            }

            return results;
        }
    }
}