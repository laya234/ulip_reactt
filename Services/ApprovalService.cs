using ULIP_proj.DTOs;
using ULIP_proj.Interfaces;
using ULIP_proj.Models;
using ULIP_proj.Enums;
using ULIP_proj.Extensions;

namespace ULIP_proj.Services
{
    public class ApprovalService : IApprovalService
    {
        private readonly IApprovalRepository _approvalRepository;
        private readonly IPolicyRepository _policyRepository;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;

        public ApprovalService(IApprovalRepository approvalRepository, IPolicyRepository policyRepository, IEmailService emailService, IUserRepository userRepository)
        {
            _approvalRepository = approvalRepository;
            _policyRepository = policyRepository;
            _emailService = emailService;
            _userRepository = userRepository;
        }

        public async Task<bool> SubmitApprovalRequestAsync(int requestId, string requestType, string reason, int userId)
        {

            var approval = new Approval
            {
                RequestId = requestId,
                RequestType = Enum.Parse<ApprovalType>(requestType),
                RequestedBy = userId,
                Status = ApprovalStatus.Pending,
                RequestReason = reason,
                RequestedAt = DateTimeExtensions.NowIst()
            };

            await _approvalRepository.CreateAsync(approval);
            return true;
        }

        public async Task<bool> ApproveRequestAsync(int approvalId, string comments, int managerId)
        {
            var approval = await _approvalRepository.GetByIdAsync(approvalId);

            if (approval == null) return false;

            approval.Status = ApprovalStatus.Approved;
            approval.ApprovedBy = managerId;
            approval.ApprovalComments = comments;
            approval.ApprovedAt = DateTimeExtensions.NowIst();

            await _approvalRepository.UpdateAsync(approval);

            if (approval.RequestType == ApprovalType.PolicySurrender)
            {
                var policy = await _policyRepository.GetByIdAsync(approval.RequestId);
                if (policy != null)
                {
                    policy.PolicyStatus = PolicyStatus.Surrendered;
                    policy.SurrenderStatus = SurrenderStatus.Processed;
                    if (policy.SurrenderValue == null)
                    {
                        policy.SurrenderValue = policy.CurrentValue * 0.90m;
                    }
                    await _policyRepository.UpdateAsync(policy);
                    
                    var customer = await _userRepository.GetByIdAsync(policy.UserId);
                    if (customer != null)
                    {
                        await _emailService.SendApprovalNotificationEmailAsync(
                            customer.Email,
                            $"{customer.FirstName} {customer.LastName}",
                            "Policy Surrender",
                            true,
                            comments
                        );
                    }
                }
            }

            return true;
        }

        public async Task<bool> RejectRequestAsync(int approvalId, string comments, int managerId)
        {
            var approval = await _approvalRepository.GetByIdAsync(approvalId);

            if (approval == null) return false;

            approval.Status = ApprovalStatus.Rejected;
            approval.ApprovedBy = managerId;
            approval.ApprovalComments = comments;
            approval.ApprovedAt = DateTimeExtensions.NowIst();

            await _approvalRepository.UpdateAsync(approval);
            
            if (approval.RequestType == ApprovalType.PolicySurrender)
            {
                var policy = await _policyRepository.GetByIdAsync(approval.RequestId);
                if (policy != null)
                {
                    var customer = await _userRepository.GetByIdAsync(policy.UserId);
                    if (customer != null)
                    {
                        await _emailService.SendApprovalNotificationEmailAsync(
                            customer.Email,
                            $"{customer.FirstName} {customer.LastName}",
                            "Policy Surrender",
                            false,
                            comments
                        );
                    }
                }
            }
            
            return true;
        }

        public async Task<IEnumerable<ApprovalDto>> GetPendingApprovalsAsync()
        {
            var approvals = await _approvalRepository.GetPendingAsync();
            return approvals.Select(a => new ApprovalDto
            {
                ApprovalId = a.ApprovalId,
                RequestId = a.RequestId,
                RequestType = a.RequestType,
                RequestedBy = a.RequestedBy,
                RequestedByName = a.RequestedByUser?.FirstName + " " + a.RequestedByUser?.LastName,
                RequestedByEmail = a.RequestedByUser?.Email,
                Status = a.Status,
                Amount = a.Amount,
                RequestReason = a.RequestReason,
                RequestedAt = a.RequestedAt
            });
        }




    }
}
