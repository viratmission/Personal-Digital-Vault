
using PersonalDigitalVault.Models;

namespace PersonalDigitalVault.Interfaces
{
    public interface IFolderRepository
    {
        Task<Folder> CreateAsync(Folder folder);
        Task<List<Folder>> GetByUserIdAsync(int userId);
        Task<Folder?> GetByIdAndUserIdAsync(int folderId, int userId);
        Task<Folder> UpdateAsync(Folder folder);
        Task DeleteAsync(Folder folder);
    }
}
