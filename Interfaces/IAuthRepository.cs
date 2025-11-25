using ULIP_proj.Models;

namespace ULIP_proj.Interfaces
{
    public interface IAuthRepository
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<User> CreateUserAsync(User user);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> PanExistsAsync(string panNumber);
    }
}
