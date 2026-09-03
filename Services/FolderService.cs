using PersonalDigitalVault.DTOs;
using PersonalDigitalVault.Interfaces;
using PersonalDigitalVault.Models;


namespace PersonalDigitalVault.Services
{
    public class FolderService : IFolderService
    {
        private readonly IFolderRepository _folderRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly IWebHostEnvironment _environment;

        public FolderService(IFolderRepository folderRepository, IDocumentRepository documentRepository,
    IWebHostEnvironment environment)
        {
            _folderRepository = folderRepository;
            _documentRepository = documentRepository;
            _environment = environment;
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

            var documents =
                await _documentRepository
                    .GetByFolderIdAndUserIdAsync(
                        folderId,
                        userId);

            foreach (var document in documents)
            {
                var fullPath = Path.Combine(
                    _environment.ContentRootPath,
                    document.FilePath);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }

            await _folderRepository.DeleteAsync(folder);

            return true;
        }
    }
}
