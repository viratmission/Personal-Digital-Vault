using PersonalDigitalVault.DTOs;

namespace PersonalDigitalVault.Interfaces
{
    public interface IAdminService
    {
        Task<List<AdminUserDto>> GetAllUsersAsync();

        Task<AdminDashboardDto> GetDashboardAsync();

        Task<bool> DeleteUserAsync(int userId);
    }
}