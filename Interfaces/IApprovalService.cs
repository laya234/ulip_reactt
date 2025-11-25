using ULIP_proj.DTOs;

namespace ULIP_proj.Interfaces
{
    public interface IApprovalService
    {
        Task<bool> SubmitApprovalRequestAsync(int requestId, string requestType, string reason, int userId);
        Task<bool> ApproveRequestAsync(int approvalId, string comments, int managerId);
        Task<bool> RejectRequestAsync(int approvalId, string comments, int managerId);
        Task<IEnumerable<ApprovalDto>> GetPendingApprovalsAsync();
    }
}
