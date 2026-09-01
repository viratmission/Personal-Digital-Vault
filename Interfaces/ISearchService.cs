using PersonalDigitalVault.DTOs;

namespace PersonalDigitalVault.Interfaces
{
    public interface ISearchService
    {
        Task<List<SearchResultDto>> SearchAsync(
            int userId,
            string? searchTerm,
            string? itemType
        );
    }
}