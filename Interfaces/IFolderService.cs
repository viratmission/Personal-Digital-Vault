using PersonalDigitalVault.DTOs;
using PersonalDigitalVault.Models;

namespace PersonalDigitalVault.Interfaces
{
    public interface IFolderService
    {
        Task<Folder> CreateFolderAsync(
            CreateFolderDto createFolderDto,
            int userId);

        Task<List<Folder>> GetFoldersByUserIdAsync(int userId);

        Task<Folder?> RenameFolderAsync(
            int folderId,
            RenameFolderDto renameFolderDto,
            int userId);
        Task<bool> DeleteFolderAsync(int folderId, int userId);
    }
}
