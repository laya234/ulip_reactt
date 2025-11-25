using Microsoft.EntityFrameworkCore;
using ULIP_proj.Data;
using ULIP_proj.Interfaces;
using ULIP_proj.Models;

namespace ULIP_proj.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ULIPDbContext _context;

        public AuthRepository(ULIPDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> PanExistsAsync(string panNumber)
        {
            return await _context.Users.AnyAsync(u => u.PanNumber == panNumber);
        }
    }
}
