using ULIP_proj.Models;

namespace ULIP_proj.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int userId);
        Task<User> GetByEmailAsync(string email);
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<IEnumerable<User>> GetAllAsync();
        Task<IEnumerable<User>> GetAgentsAsync();
        Task<IEnumerable<User>> GetManagersAsync();
    }
}
