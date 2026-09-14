using PersonalDigitalVault.DTOs;
using PersonalDigitalVault.Interfaces;
using PersonalDigitalVault.Models;

namespace PersonalDigitalVault.Services
{
    public class CredentialService : ICredentialService
    {
        private readonly ICredentialRepository _credentialRepository;
        private readonly IAesEncryptionService _aesEncryptionService;

        public CredentialService(
            ICredentialRepository credentialRepository,
            IAesEncryptionService aesEncryptionService)
        {
            _credentialRepository = credentialRepository;
            _aesEncryptionService = aesEncryptionService;
        }

        public async Task<CredentialResponseDto> CreateAsync(
            int userId,
            CreateCredentialDto dto)
        {
            var encryptedValue =
                _aesEncryptionService.Encrypt(dto.SensitiveValue);

            var credential = new Credential
            {
                EncryptedValue = encryptedValue,
                UserId = userId
            };

            var createdCredential =
                await _credentialRepository.AddAsync(credential);

            return ToMaskedResponse(createdCredential);
        }

        public async Task<List<CredentialResponseDto>> GetAllAsync(
            int userId)
        {
            var credentials =
                await _credentialRepository.GetAllByUserIdAsync(userId);

            return credentials
                .Select(ToMaskedResponse)
                .ToList();
        }

        public async Task<CredentialResponseDto?> GetByIdAsync(
            int credentialId,
            int userId)
        {
            var credential =
                await _credentialRepository.GetByIdAndUserIdAsync(
                    credentialId,
                    userId);

            if (credential == null)
            {
                return null;
            }

            return ToMaskedResponse(credential);
        }

        public async Task<CredentialResponseDto?> UpdateAsync(
            int credentialId,
            int userId,
            UpdateCredentialDto dto)
        {
            var credential =
                await _credentialRepository.GetByIdAndUserIdAsync(
                    credentialId,
                    userId);

            if (credential == null)
            {
                return null;
            }

            credential.EncryptedValue =
                _aesEncryptionService.Encrypt(dto.SensitiveValue);

            var updatedCredential =
                await _credentialRepository.UpdateAsync(credential);

            return ToMaskedResponse(updatedCredential);
        }

        public async Task<bool> DeleteAsync(
            int credentialId,
            int userId)
        {
            var credential =
                await _credentialRepository.GetByIdAndUserIdAsync(
                    credentialId,
                    userId);

            if (credential == null)
            {
                return false;
            }

            return await _credentialRepository.DeleteAsync(credential);
        }

        private CredentialResponseDto ToMaskedResponse(
            Credential credential)
        {
            return new CredentialResponseDto
            {
                Id = credential.Id,
                SensitiveValue = "********"
            };
        }
    }
}