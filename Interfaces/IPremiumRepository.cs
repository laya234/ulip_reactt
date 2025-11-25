using ULIP_proj.Models;

namespace ULIP_proj.Interfaces
{
    public interface IPremiumRepository
    {
        Task<Premium> CreateAsync(Premium premium);
        Task<Premium> UpdateAsync(Premium premium);
        Task<IEnumerable<Premium>> GetByPolicyIdAsync(int policyId);
        Task<IEnumerable<Premium>> GetOverdueAsync();
    }
}
