using ULIP_proj.DTOs;
using ULIP_proj.Interfaces;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace ULIP_proj.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto> GetUserProfileAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;
            
            return new UserDto
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                Address = user.Address,
                PanNumber = user.PanNumber,
                Role = user.Role,
                TotalCommissionEarned = user.TotalCommissionEarned,
                PoliciesSold = user.PoliciesSold,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<decimal> GetAgentCommissionAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user?.TotalCommissionEarned ?? 0;
        }

        public async Task<UserDto> UpdateProfileAsync(UpdateProfileDto updateDto, int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;
            
            user.FirstName = updateDto.FirstName;
            user.LastName = updateDto.LastName;
            user.PhoneNumber = updateDto.PhoneNumber;
            user.Address = updateDto.Address;
            
            var updatedUser = await _userRepository.UpdateAsync(user);
            return MapToUserDto(updatedUser);
        }



        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToUserDto).ToList();
        }

        public async Task<List<UserDto>> GetUsersByRoleAsync(string role)
        {
            var users = await _userRepository.GetAllAsync();
            var filteredUsers = users.Where(u => u.Role.ToString() == role);
            return filteredUsers.Select(MapToUserDto).ToList();
        }

        public async Task<bool> UploadDocumentAsync(int userId, IFormFile file, string documentType)
        {
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "documents", userId.ToString());
            Directory.CreateDirectory(uploadsPath);
            
            var fileName = $"{documentType}_{DateTime.Now:yyyyMMddHHmmss}_{file.FileName}";
            var filePath = Path.Combine(uploadsPath, fileName);
            
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            
            return true;
        }

        public async Task<List<DocumentDto>> GetUserDocumentsAsync(int userId)
        {
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "documents", userId.ToString());
            
            if (!Directory.Exists(uploadsPath))
                return new List<DocumentDto>();
            
            var files = Directory.GetFiles(uploadsPath);
            var documents = new List<DocumentDto>();
            
            for (int i = 0; i < files.Length; i++)
            {
                var fileInfo = new FileInfo(files[i]);
                var parts = fileInfo.Name.Split('_');
                var documentType = parts.Length > 0 ? parts[0] : "Unknown";
                
                documents.Add(new DocumentDto
                {
                    Id = i + 1,
                    FileName = fileInfo.Name,
                    DocumentType = documentType,
                    UploadDate = fileInfo.CreationTime.ToString("yyyy-MM-dd"),
                    Size = $"{(fileInfo.Length / 1024.0 / 1024.0):F2} MB"
                });
            }
            
            return documents;
        }

        public async Task<byte[]> DownloadDocumentAsync(int documentId, int userId)
        {
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "documents", userId.ToString());
            
            if (!Directory.Exists(uploadsPath))
                return null;
            
            var files = Directory.GetFiles(uploadsPath);
            if (documentId <= 0 || documentId > files.Length)
                return null;
            
            var filePath = files[documentId - 1];
            return await File.ReadAllBytesAsync(filePath);
        }

        public async Task<UserDto> GetUserByPhoneAsync(string phoneNumber)
        {
            var users = await _userRepository.GetAllAsync();
            var user = users.FirstOrDefault(u => u.PhoneNumber == phoneNumber);
            return user != null ? MapToUserDto(user) : null;
        }

        private UserDto MapToUserDto(Models.User user)
        {
            return new UserDto
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                Address = user.Address,
                PanNumber = user.PanNumber,
                Role = user.Role,
                TotalCommissionEarned = user.TotalCommissionEarned,
                PoliciesSold = user.PoliciesSold,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
