using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PersonalDigitalVault.DTOs;
using PersonalDigitalVault.Interfaces;
using PersonalDigitalVault.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PersonalDigitalVault.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;

        public AuthService(
            IUserRepository userRepository,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            bool emailExists =
                await _userRepository.EmailExistsAsync(registerDto.Email);

            if (emailExists)
            {
                return false;
            }

            var user = new User
            {
                FullName = registerDto.FullName,
                Email = registerDto.Email
            };

            user.PasswordHash =
                _passwordHasher.HashPassword(
                    user,
                    registerDto.Password
                );

            await _userRepository.AddUserAsync(user);

            return true;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var user =
                await _userRepository.GetUserByEmailAsync(loginDto.Email);

            if (user == null)
            {
                return null;
            }

            var passwordResult =
                _passwordHasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    loginDto.Password
                );

            if (passwordResult == PasswordVerificationResult.Failed)
            {
                return null;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!
                )
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            int expiryMinutes =
                int.Parse(_configuration["Jwt:ExpiryMinutes"]!);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            string tokenString =
                new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginResponseDto
            {
                Token = tokenString
            };
        }
        public async Task<ProfileDto?> GetProfileAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            return new ProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email
            };
        }
        public async Task<bool> UpdateProfileAsync(
            int userId,UpdateProfileDto updateProfileDto)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            var existingUser =
                await _userRepository.GetUserByEmailAsync(updateProfileDto.Email);

            if (existingUser != null && existingUser.Id != userId)
            {
                return false;
            }

            user.FullName = updateProfileDto.FullName;
            user.Email = updateProfileDto.Email;

            await _userRepository.UpdateUserAsync(user);

            return true;
        }
    }
}