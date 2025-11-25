using ULIP_proj.Models;

namespace ULIP_proj.Interfaces
{
    public interface IPolicyRepository
    {
        Task<Policy> GetByIdAsync(int policyId);
        Task<Policy> CreateAsync(Policy policy);
        Task<Policy> UpdateAsync(Policy policy);
        Task<IEnumerable<Policy>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<Policy>> GetByAgentIdAsync(int agentId);
        Task<IEnumerable<Policy>> GetSurrenderRequestsAsync();
        Task<IEnumerable<Policy>> GetAllAsync();
    }
}
