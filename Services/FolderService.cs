using PersonalDigitalVault.DTOs;
using PersonalDigitalVault.Interfaces;
using PersonalDigitalVault.Models;


namespace PersonalDigitalVault.Services
{
    public class FolderService : IFolderService
    {
        private readonly IFolderRepository _folderRepository;

        public FolderService(IFolderRepository folderRepository)
        {
            _folderRepository = folderRepository;
        }

        public async Task<Folder> CreateFolderAsync(
            CreateFolderDto createFolderDto,
            int userId)
        {
            var folder = new Folder
            {
                Name = createFolderDto.Name.Trim(),
                UserId = userId
            };
            return await _folderRepository.CreateAsync(folder);
        }

        public async Task<List<Folder>> GetFoldersByUserIdAsync(int userId)
        {
            return await _folderRepository.GetByUserIdAsync(userId);

        }

        public async Task<Folder?> RenameFolderAsync(
            int folderId,
            RenameFolderDto renameFolderDto,
            int userId)
        {
            var folder = await _folderRepository
                .GetByIdAndUserIdAsync(folderId, userId);

            if (folder == null)
            {
                return null;
            }
            folder.Name = renameFolderDto.Name.Trim();

            return await _folderRepository.UpdateAsync(folder);


        }

        public async Task<bool> DeleteFolderAsync(
            int folderId,
            int userId)
        {
            var folder = await _folderRepository
                .GetByIdAndUserIdAsync(folderId, userId);

            if (folder == null)
            {
                return false;
            }

            await _folderRepository.DeleteAsync(folder);
            return true;

        }
    }
}
