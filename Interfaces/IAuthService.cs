using PersonalDigitalVault.DTOs;

namespace PersonalDigitalVault.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterDto registerDto);

        Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);

        Task<ProfileDto?> GetProfileAsync(int userId);

        Task<bool> UpdateProfileAsync(int userId, UpdateProfileDto updateProfileDto);
    }
}