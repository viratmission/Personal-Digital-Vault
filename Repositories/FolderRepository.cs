using Microsoft.EntityFrameworkCore;
using PersonalDigitalVault.Data;
using PersonalDigitalVault.Interfaces;
using PersonalDigitalVault.Models;


namespace PersonalDigitalVault.Repositories
{
    public class FolderRepository: IFolderRepository
    {
         private readonly AppDbContext _context;
        
        public FolderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Folder> CreateAsync(Folder folder)
        {
            _context.Folders.Add(folder);
            await _context.SaveChangesAsync();
            return folder;
        }
        
        public async Task<List<Folder>> GetByUserIdAsync(int userId)
        {
            return await _context.Folders
                .Where(f => f.UserId == userId)
                .ToListAsync();
        }

        public async Task<Folder?> GetByIdAndUserIdAsync(
            int folderId,
            int userId)
        {
            return await _context.Folders
                .FirstOrDefaultAsync(f =>
                f.Id == folderId &&
                f.UserId == userId);
        }

        public async Task<Folder> UpdateAsync(Folder folder)
        {
            _context.Folders.Update(folder);
            await _context.SaveChangesAsync();

            return folder;
        }
        public async Task DeleteAsync(Folder folder)
        {
            _context.Folders.Remove(folder);
            await _context.SaveChangesAsync();
        }
    }
}
