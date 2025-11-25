using Microsoft.EntityFrameworkCore;
using ULIP_proj.Data;
using ULIP_proj.Interfaces;
using ULIP_proj.Models;
using ULIP_proj.Enums;

namespace ULIP_proj.Repositories
{
    public class ApprovalRepository : IApprovalRepository
    {
        private readonly ULIPDbContext _context;

        public ApprovalRepository(ULIPDbContext context)
        {
            _context = context;
        }

        public async Task<Approval> CreateAsync(Approval approval)
        {
            _context.Approvals.Add(approval);
            await _context.SaveChangesAsync();
            return approval;
        }

        public async Task<Approval> UpdateAsync(Approval approval)
        {
            _context.Approvals.Update(approval);
            await _context.SaveChangesAsync();
            return approval;
        }

        public async Task<IEnumerable<Approval>> GetPendingAsync()
        {
            return await _context.Approvals
                .Where(a => a.Status == ApprovalStatus.Pending)
                .Include(a => a.RequestedByUser)
                .OrderBy(a => a.RequestedAt)
                .ToListAsync();
        }

        public async Task<Approval> GetByIdAsync(int approvalId)
        {
            return await _context.Approvals
                .Include(a => a.RequestedByUser)
                .FirstOrDefaultAsync(a => a.ApprovalId == approvalId);
        }
    }
}
