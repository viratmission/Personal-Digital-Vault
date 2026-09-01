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
        private readonly IFileEncryptionService _fileEncryptionService;
        private readonly IFileHashService _fileHashService;

        public DocumentService(
            IDocumentRepository documentRepository,
            IFolderRepository folderRepository,
            IConfiguration configuration,
            IWebHostEnvironment environment,
            IFileEncryptionService fileEncryptionService,
            IFileHashService fileHashService)
        {
            _documentRepository = documentRepository;
            _folderRepository = folderRepository;
            _configuration = configuration;
            _environment = environment;
            _fileEncryptionService = fileEncryptionService;
            _fileHashService = fileHashService;
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

            using (var inputStream =
                uploadDocumentDto.File.OpenReadStream())
            using (var outputStream = new FileStream(
                fullFilePath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None))
            {
                await _fileEncryptionService.EncryptAsync(
                    inputStream,
                    outputStream);
            }

            string sha256Hash;

            using (var encryptedFileStream = new FileStream(
                fullFilePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read))
            {
                sha256Hash =
                    await _fileHashService.GenerateSha256Async(
                        encryptedFileStream);
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
                FolderId = uploadDocumentDto.FolderId,
                Sha256Hash = sha256Hash
            };

            return await _documentRepository.CreateAsync(document);
        }

        public async Task<List<Document>>
            GetDocumentsByUserIdAsync(int userId)
        {
            return await _documentRepository
                .GetByUserIdAsync(userId);
        }
        public async Task<(Document? Document, byte[]? FileBytes)>
            GetDocumentForDownloadAsync(
                int documentId,
                int userId)
        {
            var document = await _documentRepository
                .GetByIdAndUserIdAsync(
                    documentId,
                    userId);

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

            if (string.IsNullOrWhiteSpace(document.Sha256Hash))
            {
                throw new InvalidDataException(
                    "File integrity hash is missing.");
            }

            using var encryptedStream = new FileStream(
                fullPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);

            var currentHash =
                await _fileHashService.GenerateSha256Async(
                    encryptedStream);

            var hashMatches = string.Equals(
                currentHash,
                document.Sha256Hash,
                StringComparison.OrdinalIgnoreCase);

            if (!hashMatches)
            {
                throw new InvalidDataException(
                    "File integrity check failed.");
            }

            encryptedStream.Position = 0;

            using var decryptedStream =
                new MemoryStream();

            await _fileEncryptionService.DecryptAsync(
                encryptedStream,
                decryptedStream);

            return (
                document,
                decryptedStream.ToArray());
        }

        public async Task<Document?> RenameDocumentAsync(
            int documentId,
            RenameDocumentDto renameDocumentDto,
            int userId)
        {
            var document = await _documentRepository
                .GetByIdAndUserIdAsync(
                    documentId,
                    userId);

            if (document == null)
            {
                return null;
            }

            document.FileName =
                renameDocumentDto.FileName.Trim();

            return await _documentRepository
                .UpdateAsync(document);
        }

        public async Task<bool> DeleteDocumentAsync(
            int documentId,
            int userId)
        {
            var document = await _documentRepository
                .GetByIdAndUserIdAsync(
                    documentId,
                    userId);

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