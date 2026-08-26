using PersonalDigitalVault.DTOs;
using PersonalDigitalVault.Interfaces;
using PersonalDigitalVault.Models;

namespace PersonalDigitalVault.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IFolderRepository _folderRepository;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public DocumentService(
            IDocumentRepository documentRepository,
            IFolderRepository folderRepository,
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            _documentRepository = documentRepository;
            _folderRepository = folderRepository;
            _configuration = configuration;
            _environment = environment;
        }

        public async Task<Document?> UploadDocumentAsync(
            UploadDocumentDto uploadDocumentDto,
            int userId)
        {
            var folder = await _folderRepository.GetByIdAndUserIdAsync(
                uploadDocumentDto.FolderId,
                userId);

            if (folder == null)
            {
                return null;
            }

            if (uploadDocumentDto.File == null ||
                uploadDocumentDto.File.Length == 0)
            {
                throw new ArgumentException("Uploaded file is empty.");
            }

            var storagePath = _configuration["DocumentStorage:Path"];

            if (string.IsNullOrWhiteSpace(storagePath))
            {
                throw new InvalidOperationException(
                    "Document storage path is not configured.");
            }

            var fullStoragePath = Path.Combine(
                _environment.ContentRootPath,
                storagePath);

            Directory.CreateDirectory(fullStoragePath);

            var originalFileName = Path.GetFileName(
                uploadDocumentDto.File.FileName);

            var extension = Path.GetExtension(originalFileName);

            var storedFileName =
                $"{Guid.NewGuid():N}{extension}";

            var fullFilePath = Path.Combine(
                fullStoragePath,
                storedFileName);

            using (var stream = new FileStream(
                fullFilePath,
                FileMode.CreateNew))
            {
                await uploadDocumentDto.File.CopyToAsync(stream);
            }

            var document = new Document
            {
                FileName = originalFileName,
                StoredFileName = storedFileName,
                FilePath = Path.Combine(
                    storagePath,
                    storedFileName),
                ContentType = uploadDocumentDto.File.ContentType,
                FileSize = uploadDocumentDto.File.Length,
                UploadedAt = DateTime.UtcNow,
                UserId = userId,
                FolderId = uploadDocumentDto.FolderId
            };

            return await _documentRepository.CreateAsync(document);
        }
        public async Task<List<Document>> GetDocumentsByUserIdAsync(int userId)
        {
            return await _documentRepository.GetByUserIdAsync(userId);
        }
        public async Task<(Document? Document, string? FullPath)>
          GetDocumentForDownloadAsync(
               int documentId,
               int userId)
        {
            var document = await _documentRepository
                .GetByIdAndUserIdAsync(documentId, userId);

            if (document == null)
            {
                return (null, null);
            }

            var fullPath = Path.Combine(
                _environment.ContentRootPath,
                document.FilePath);

            if (!File.Exists(fullPath))
            {
                return (document, null);
            }

            return (document, fullPath);
        }
        public async Task<Document?> RenameDocumentAsync(
    int documentId,
    RenameDocumentDto renameDocumentDto,
    int userId)
        {
            var document = await _documentRepository
                .GetByIdAndUserIdAsync(documentId, userId);

            if (document == null)
            {
                return null;
            }

            document.FileName = renameDocumentDto.FileName.Trim();

            return await _documentRepository.UpdateAsync(document);
        }
        public async Task<bool> DeleteDocumentAsync(
    int documentId,
    int userId)
        {
            var document = await _documentRepository
                .GetByIdAndUserIdAsync(documentId, userId);

            if (document == null)
            {
                return false;
            }

            var fullPath = Path.Combine(
                _environment.ContentRootPath,
                document.FilePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            await _documentRepository.DeleteAsync(document);

            return true;
        }
    }
}