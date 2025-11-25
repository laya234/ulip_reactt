using ULIP_proj.DTOs;
using Microsoft.AspNetCore.Http;

namespace ULIP_proj.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetUserProfileAsync(int userId);
        Task<decimal> GetAgentCommissionAsync(int userId);
        Task<UserDto> UpdateProfileAsync(UpdateProfileDto updateDto, int userId);
        Task<List<UserDto>> GetAllUsersAsync();
        Task<List<UserDto>> GetUsersByRoleAsync(string role);
        Task<bool> UploadDocumentAsync(int userId, IFormFile file, string documentType);
        Task<List<DocumentDto>> GetUserDocumentsAsync(int userId);
        Task<byte[]> DownloadDocumentAsync(int documentId, int userId);
        Task<UserDto> GetUserByPhoneAsync(string phoneNumber);
    }
}
