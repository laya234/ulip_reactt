using ULIP_proj.DTOs;

namespace ULIP_proj.Interfaces
{
    public interface IPremiumService
    {
        Task<bool> ProcessPremiumPaymentAsync(PayPremiumDto premiumDto);
        Task<decimal> CalculateCommissionAsync(int policyId, decimal premiumAmount);
        Task<IEnumerable<PremiumDto>> GetOverduePremumsAsync();
        Task<bool> SendReminderAsync(int policyId);
        Task<bool> GeneratePremiumScheduleAsync(int policyId);
        Task<IEnumerable<PremiumDto>> GetPremiumScheduleAsync(int policyId);
        Task<bool> ProcessOverduePoliciesAsync();
    }
}
