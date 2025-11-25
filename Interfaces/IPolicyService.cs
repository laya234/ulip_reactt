using ULIP_proj.DTOs;

namespace ULIP_proj.Interfaces
{
    public interface IPolicyService
    {
        Task<PolicyDto> CreatePolicyAsync(CreatePolicyDto policyDto, int agentId); 
        Task<PolicyDto> GetPolicyDetailsAsync(int policyId);
        Task<bool> RequestSurrenderAsync(int policyId, string reason, int userId);
        Task<decimal> CalculateSurrenderValueAsync(int policyId);
        Task<IEnumerable<PolicyDto>> GetMyPoliciesAsync(int userId); 
        Task<IEnumerable<PolicyDto>> GetAgentPoliciesAsync(int agentId);
        Task<PolicyProposalDto> CreatePolicyProposalAsync(int saleId, CreatePolicyProposalDto proposalDto, int agentId);
        Task<PolicyProposalDto> CreatePolicyProposalForCustomerAsync(int customerId, CreatePolicyProposalDto proposalDto, int agentId);
        Task<IEnumerable<PolicyProposalDto>> GetPendingProposalsAsync(int userId);
        Task<bool> AcceptPolicyProposalAsync(int policyId, PolicyAcceptanceDto acceptanceDto);
        Task<byte[]> GeneratePolicyStatementAsync(int policyId, int userId);
        Task<bool> ProcessSurrenderApprovalAsync(int policyId, bool approved, int managerId, string? comments = null);

        Task<bool> UploadPolicyDocumentAsync(int policyId, IFormFile file, string documentType, int userId);
        Task<bool> VerifyDocumentAsync(int policyId, string documentType, bool approved, int verifierId, string? comments = null);
        Task<IEnumerable<object>> GetPoliciesWithPendingDocumentsAsync();
        Task<object> GetDocumentStatusAsync(int policyId); 
    }
}
