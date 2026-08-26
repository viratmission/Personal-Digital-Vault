using Microsoft.EntityFrameworkCore;
using PersonalDigitalVault.Data;
using PersonalDigitalVault.Interfaces;
using PersonalDigitalVault.Models;

namespace PersonalDigitalVault.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly AppDbContext _context;

        public DocumentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Document> CreateAsync(Document document)
        {
            _context.Documents.Add(document);

            await _context.SaveChangesAsync();

            return document;
        }

        public async Task<List<Document>> GetByUserIdAsync(int userId)
        {
            return await _context.Documents
                .Where(d => d.UserId == userId)
                .ToListAsync();
        }
        public async Task<Document?> GetByIdAndUserIdAsync(
            int documentId,
            int userId)
        {
            return await _context.Documents
                .FirstOrDefaultAsync(d =>
                    d.Id == documentId &&
                    d.UserId == userId);
        }
        public async Task<Document> UpdateAsync(Document document)
        {
            _context.Documents.Update(document);

            await _context.SaveChangesAsync();

            return document;
        }
        public async Task DeleteAsync(Document document)
        {
            _context.Documents.Remove(document);

            await _context.SaveChangesAsync();
        }
    }
}