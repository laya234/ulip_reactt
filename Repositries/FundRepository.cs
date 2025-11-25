using Microsoft.EntityFrameworkCore;
using ULIP_proj.Data;
using ULIP_proj.Interfaces;
using ULIP_proj.Models;

namespace ULIP_proj.Repositories
{
    public class FundRepository : IFundRepository
    {
        private readonly ULIPDbContext _context;

        public FundRepository(ULIPDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Fund>> GetAllAsync()
        {
            return await _context.Funds.ToListAsync();
        }

        public async Task<Fund> GetByIdAsync(int fundId)
        {
            return await _context.Funds.FindAsync(fundId);
        }

        public async Task<Fund> CreateAsync(Fund fund)
        {
            _context.Funds.Add(fund);
            await _context.SaveChangesAsync();
            return fund;
        }

        public async Task<Fund> UpdateNAVAsync(int fundId, decimal newNAV)
        {
            var fund = await _context.Funds.FindAsync(fundId);
            if (fund != null)
            {
                fund.CurrentNAV = newNAV;
                fund.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            return fund;
        }
    }
}
