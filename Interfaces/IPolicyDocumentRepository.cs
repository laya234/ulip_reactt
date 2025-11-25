using ULIP_proj.Models;

namespace ULIP_proj.Interfaces
{
    public interface IPolicyDocumentRepository
    {
        Task<PolicyDocument> CreateAsync(PolicyDocument document);
        Task<IEnumerable<PolicyDocument>> GetByPolicyIdAsync(int policyId);
        Task<IEnumerable<PolicyDocument>> GetUploadedDocumentsAsync();
        Task<PolicyDocument?> GetByPolicyIdAndTypeAsync(int policyId, string documentType);
        Task<PolicyDocument> UpdateAsync(PolicyDocument document);
    }
}
