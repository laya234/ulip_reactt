using Microsoft.EntityFrameworkCore;
using ULIP_proj.Data;
using ULIP_proj.Interfaces;
using ULIP_proj.Models;
using ULIP_proj.Enums;

namespace ULIP_proj.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        private readonly ULIPDbContext _context;

        public SaleRepository(ULIPDbContext context)
        {
            _context = context;
        }

        public async Task<Sale> CreateAsync(Sale sale)
        {
            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();
            return sale;
        }

        public async Task<Sale> UpdateAsync(Sale sale)
        {
            _context.Sales.Update(sale);
            await _context.SaveChangesAsync();
            return sale;
        }

        public async Task<IEnumerable<Sale>> GetByAgentIdAsync(int agentId)
        {
            return await _context.Sales
                .Where(s => s.AgentId == agentId)
                .OrderByDescending(s => s.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Sale>> GetLeadsAsync()
        {
            return await _context.Sales
                .Where(s => s.Status == SaleStatus.Lead || s.Status == SaleStatus.Quoted)
                .Include(s => s.Agent)
                .OrderByDescending(s => s.CreatedDate)
                .ToListAsync();
        }
    }
}
