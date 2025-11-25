using Microsoft.EntityFrameworkCore;
using ULIP_proj.Data;
using ULIP_proj.Interfaces;
using ULIP_proj.Models;

namespace ULIP_proj.Repositories
{
    public class PolicyRepository : IPolicyRepository
    {
        private readonly ULIPDbContext _context;

        public PolicyRepository(ULIPDbContext context)
        {
            _context = context;
        }

        public async Task<Policy> GetByIdAsync(int policyId)
        {
            return await _context.Policies
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PolicyId == policyId);
        }

        public async Task<Policy> CreateAsync(Policy policy)
        {
            _context.Policies.Add(policy);
            await _context.SaveChangesAsync();
            return policy;
        }

        public async Task<Policy> UpdateAsync(Policy policy)
        {
            _context.Policies.Update(policy);
            await _context.SaveChangesAsync();
            return policy;
        }

        public async Task<IEnumerable<Policy>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.Policies
                .Where(p => p.UserId == customerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Policy>> GetByAgentIdAsync(int agentId)
        {
            return await _context.Policies
                .Where(p => p.AgentId == agentId)
                .Include(p => p.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Policy>> GetSurrenderRequestsAsync()
        {
            return await _context.Policies
                .Where(p => p.SurrenderRequested == true)
                .Include(p => p.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Policy>> GetAllAsync()
        {
            return await _context.Policies
                .Include(p => p.User)
                .ToListAsync();
        }
    }
}
