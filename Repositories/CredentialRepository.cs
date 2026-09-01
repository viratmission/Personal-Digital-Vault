using Microsoft.EntityFrameworkCore;
using PersonalDigitalVault.Data;
using PersonalDigitalVault.Interfaces;
using PersonalDigitalVault.Models;

namespace PersonalDigitalVault.Repositories
{
    public class CredentialRepository : ICredentialRepository
    {
        private readonly AppDbContext _context;

        public CredentialRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Credential> AddAsync(
            Credential credential)
        {
            _context.Credentials.Add(credential);

            await _context.SaveChangesAsync();

            return credential;
        }

        public async Task<List<Credential>> GetAllByUserIdAsync(
            int userId)
        {
            return await _context.Credentials
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<Credential?> GetByIdAndUserIdAsync(
            int credentialId,
            int userId)
        {
            return await _context.Credentials
                .FirstOrDefaultAsync(c =>
                    c.Id == credentialId &&
                    c.UserId == userId);
        }

        public async Task<Credential> UpdateAsync(
            Credential credential)
        {
            _context.Credentials.Update(credential);

            await _context.SaveChangesAsync();

            return credential;
        }

        public async Task<bool> DeleteAsync(
            Credential credential)
        {
            _context.Credentials.Remove(credential);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}