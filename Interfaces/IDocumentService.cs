using PersonalDigitalVault.DTOs;
using PersonalDigitalVault.Models;

namespace PersonalDigitalVault.Interfaces
{
    public interface IDocumentService
    {
        Task<Document?> UploadDocumentAsync(
            UploadDocumentDto uploadDocumentDto,
            int userId);

        Task<List<Document>> GetDocumentsByUserIdAsync(int userId);

        Task<(Document? Document, string? FullPath)> GetDocumentForDownloadAsync(
            int documentId,
            int userId);

        Task<Document?> RenameDocumentAsync(
            int documentId,
            RenameDocumentDto renameDocumentDto,
            int userId);

        Task<bool> DeleteDocumentAsync(
            int documentId,
            int userId);
    }
}