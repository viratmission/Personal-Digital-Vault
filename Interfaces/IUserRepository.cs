using PersonalDigitalVault.Models;

namespace PersonalDigitalVault.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> EmailExistsAsync(string email);

        Task<User?> GetUserByEmailAsync(string email);

        Task<User?> GetUserByIdAsync(int id);

        Task AddUserAsync(User user);

        Task UpdateUserAsync(User user);
    }
}