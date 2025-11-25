using ULIP_proj.DTOs;

namespace ULIP_proj.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionDto> ProcessInvestmentAsync(int policyId, decimal amount, int fundId);
        Task<bool> ProcessFundSwitchAsync(int policyId, int fromFundId, int toFundId, decimal amount);
        Task<bool> RequestHighValueApprovalAsync(int transactionId);
        Task<decimal> GetPolicyCurrentValueAsync(int policyId);
        Task<IEnumerable<TransactionDto>> GetTransactionsByPolicyAsync(int policyId);
        Task<PortfolioValueDto> CalculatePortfolioValueAsync(int policyId);
    }
}
