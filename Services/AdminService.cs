using Microsoft.EntityFrameworkCore;
using PersonalDigitalVault.Data;
using PersonalDigitalVault.DTOs;
using PersonalDigitalVault.Interfaces;

namespace PersonalDigitalVault.Services
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _context;

        public AdminService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AdminUserDto>> GetAllUsersAsync()
        {
            return await _context.Users
                .Select(u => new AdminUserDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    IsAdmin = u.IsAdmin
                })
                .ToListAsync();
        }

        public async Task<AdminDashboardDto> GetDashboardAsync()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalUploads = await _context.Documents.CountAsync();
            var totalStoredFiles = await _context.Documents.CountAsync();

            return new AdminDashboardDto
            {
                TotalUsers = totalUsers,
                TotalUploads = totalUploads,
                TotalStoredFiles = totalStoredFiles
            };
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return false;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}