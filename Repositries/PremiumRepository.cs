using Microsoft.EntityFrameworkCore;
using ULIP_proj.Data;
using ULIP_proj.Interfaces;
using ULIP_proj.Models;
using ULIP_proj.Enums;

namespace ULIP_proj.Repositories
{
    public class PremiumRepository : IPremiumRepository
    {
        private readonly ULIPDbContext _context;

        public PremiumRepository(ULIPDbContext context)
        {
            _context = context;
        }

        public async Task<Premium> CreateAsync(Premium premium)
        {
            _context.Premiums.Add(premium);
            await _context.SaveChangesAsync();
            return premium;
        }

        public async Task<Premium> UpdateAsync(Premium premium)
        {
            _context.Premiums.Update(premium);
            await _context.SaveChangesAsync();
            return premium;
        }

        public async Task<IEnumerable<Premium>> GetByPolicyIdAsync(int policyId)
        {
            return await _context.Premiums
                .Where(p => p.PolicyId == policyId)
                .OrderByDescending(p => p.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Premium>> GetOverdueAsync()
        {
            return await _context.Premiums
                .Where(p => p.Status == PremiumStatus.Overdue || 
                           (p.Status == PremiumStatus.Pending && p.DueDate < DateTime.Now))
                .Include(p => p.Policy)
                .ThenInclude(p => p.User)
                .OrderByDescending(p => p.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Premium>> GetByStatusAsync(PremiumStatus status)
        {
            return await _context.Premiums
                .Where(p => p.Status == status)
                .ToListAsync();
        }
    }
}
