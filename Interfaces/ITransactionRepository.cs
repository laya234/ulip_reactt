using ULIP_proj.Models;

namespace ULIP_proj.Interfaces
{
    public interface ITransactionRepository
    {
        Task<Transaction> CreateAsync(Transaction transaction);
        Task<Transaction> UpdateAsync(Transaction transaction);
        Task<Transaction?> GetByIdAsync(int transactionId);
        Task<IEnumerable<Transaction>> GetByPolicyIdAsync(int policyId);
        Task<IEnumerable<Transaction>> GetPendingApprovalsAsync();
    }
}
