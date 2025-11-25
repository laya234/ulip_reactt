using ULIP_proj.Models;

namespace ULIP_proj.Interfaces
{
    public interface IApprovalRepository
    {
        Task<Approval> CreateAsync(Approval approval);
        Task<Approval> UpdateAsync(Approval approval);
        Task<IEnumerable<Approval>> GetPendingAsync();
        Task<Approval> GetByIdAsync(int approvalId);
    }
}
