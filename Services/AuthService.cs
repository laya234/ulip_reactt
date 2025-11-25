using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ULIP_proj.DTOs;
using ULIP_proj.Interfaces;
using ULIP_proj.Models;
using ULIP_proj.Enums;

namespace ULIP_proj.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly IEmailService _emailService;

        public AuthService(IAuthRepository authRepository, IConfiguration configuration, ILogger<AuthService> logger, IEmailService emailService)
        {
            _authRepository = authRepository;
            _configuration = configuration;
            _logger = logger;
            _emailService = emailService;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            _logger.LogInformation("Login attempt for email: {Email}", loginDto.Email);
            var user = await _authRepository.GetUserByEmailAsync(loginDto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            {
                _logger.LogWarning("Login failed - invalid credentials for email: {Email}", loginDto.Email);
                return null;
            }

            var token = GenerateJwtToken(MapToUserDto(user));
            _logger.LogInformation("Login successful for user: {UserId}", user.UserId);

            return new AuthResponseDto
            {
                Token = token,
                User = MapToUserDto(user),
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            _logger.LogInformation("Registration attempt for email: {Email}", registerDto.Email);
            if (await _authRepository.EmailExistsAsync(registerDto.Email))
            {
                _logger.LogWarning("Registration failed - email already exists: {Email}", registerDto.Email);
                return null;
            }

            if (!IsValidPAN(registerDto.PanNumber))
            {
                _logger.LogWarning("Registration failed - invalid PAN format: {PAN}", registerDto.PanNumber);
                throw new ArgumentException("Invalid PAN format. PAN should be in format: ABCDE1234F");
            }

            if (await _authRepository.PanExistsAsync(registerDto.PanNumber))
            {
                _logger.LogWarning("Registration failed - PAN already exists: {PAN}", registerDto.PanNumber);
                throw new ArgumentException("PAN number already registered");
            }

            var user = new User
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                DateOfBirth = registerDto.DateOfBirth,
                Address = registerDto.Address,
                PanNumber = registerDto.PanNumber,
                Role = registerDto.Role,
                Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password)
            };

            var createdUser = await _authRepository.CreateUserAsync(user);
            _logger.LogInformation("User registered successfully: {UserId}", createdUser.UserId);
            
            try
            {
                await _emailService.SendWelcomeEmailAsync(
                    createdUser.Email,
                    $"{createdUser.FirstName} {createdUser.LastName}",
                    createdUser.Role.ToString(),
                    registerDto.Password
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send welcome email to {Email}", createdUser.Email);
            }
            
            var token = GenerateJwtToken(MapToUserDto(createdUser));

            return new AuthResponseDto
            {
                Token = token,
                User = MapToUserDto(createdUser),
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };
        }

        private string GenerateJwtToken(UserDto user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-secret-key-here-must-be-at-least-32-characters-long"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("userId", user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                new Claim("firstName", user.FirstName),
                new Claim("lastName", user.LastName)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



        private bool IsValidPAN(string pan)
        {
            if (string.IsNullOrEmpty(pan) || pan.Length != 10)
                return false;

            return System.Text.RegularExpressions.Regex.IsMatch(pan, @"^[A-Z]{5}[0-9]{4}[A-Z]{1}$");
        }

        private UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                TotalCommissionEarned = user.TotalCommissionEarned,
                PoliciesSold = user.PoliciesSold,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
