using PersonalDigitalVault.Models;

namespace PersonalDigitalVault.Interfaces
{
    public interface ICredentialRepository
    {
        Task<Credential> AddAsync(Credential credential);

        Task<List<Credential>> GetAllByUserIdAsync(int userId);

        Task<Credential?> GetByIdAndUserIdAsync(
            int credentialId,
            int userId);

        Task<Credential> UpdateAsync(Credential credential);

        Task<bool> DeleteAsync(Credential credential);
    }
}