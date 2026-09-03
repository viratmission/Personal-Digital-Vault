using PersonalDigitalVault.Models;

namespace PersonalDigitalVault.Interfaces
{
    public interface IDocumentRepository
    {
        Task<Document> CreateAsync(Document document);

        Task<List<Document>> GetByUserIdAsync(int userId);

        Task<Document?> GetByIdAndUserIdAsync(
            int documentId,
            int userId);
        Task<List<Document>> GetByFolderIdAndUserIdAsync(int folderId,int userId);
        Task<Document> UpdateAsync(Document document);

        Task DeleteAsync(Document document);

      
    }
}