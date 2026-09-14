using PersonalDigitalVault.DTOs;

namespace PersonalDigitalVault.Interfaces
{
    public interface ICredentialService
    {
        Task<CredentialResponseDto> CreateAsync(
            int userId,
            CreateCredentialDto dto);

        Task<List<CredentialResponseDto>> GetAllAsync(
            int userId);

        Task<CredentialResponseDto?> GetByIdAsync(
            int credentialId,
            int userId);

        Task<CredentialResponseDto?> UpdateAsync(
            int credentialId,
            int userId,
            UpdateCredentialDto dto);

        Task<bool> DeleteAsync(
            int credentialId,
            int userId);
    }
}