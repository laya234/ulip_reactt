using Microsoft.EntityFrameworkCore;
using ULIP_proj.Data;
using ULIP_proj.Interfaces;
using ULIP_proj.Models;
using ULIP_proj.Enums;

namespace ULIP_proj.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ULIPDbContext _context;

        public UserRepository(ULIPDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<IEnumerable<User>> GetAgentsAsync()
        {
            return await _context.Users
                .Where(u => u.Role == UserRole.Agent)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetManagersAsync()
        {
            return await _context.Users
                .Where(u => u.Role == UserRole.Manager)
                .ToListAsync();
        }
    }
}
