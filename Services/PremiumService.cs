using ULIP_proj.DTOs;
using ULIP_proj.Interfaces;
using ULIP_proj.Models;
using ULIP_proj.Enums;
using ULIP_proj.Extensions;

namespace ULIP_proj.Services
{
    public class PremiumService : IPremiumService
    {
        private readonly IPremiumRepository _premiumRepository;
        private readonly ITransactionService _transactionService;
        private readonly IUserRepository _userRepository;
        private readonly IPolicyRepository _policyRepository;
        private readonly IEmailService _emailService;

        public PremiumService(IPremiumRepository premiumRepository, ITransactionService transactionService, IUserRepository userRepository, IPolicyRepository policyRepository, IEmailService emailService)
        {
            _premiumRepository = premiumRepository;
            _transactionService = transactionService;
            _userRepository = userRepository;
            _policyRepository = policyRepository;
            _emailService = emailService;
        }

        public async Task<bool> ProcessPremiumPaymentAsync(PayPremiumDto premiumDto)
        {
            Console.WriteLine($"[PREMIUM] Starting premium payment for Policy {premiumDto.PolicyId}, Amount {premiumDto.PremiumAmount}, Fund {premiumDto.FundId}");
            
            var policy = await _policyRepository.GetByIdAsync(premiumDto.PolicyId);
            if (policy == null)
                throw new Exception("Policy not found");

            await ValidatePaymentFrequencyAsync(premiumDto.PolicyId, policy.PremiumFrequency);
            
            var premium = new Premium
            {
                PolicyId = premiumDto.PolicyId,
                PremiumAmount = premiumDto.PremiumAmount,
                DueDate = premiumDto.DueDate,
                PaidDate = DateTimeExtensions.NowIst(),
                Status = PremiumStatus.Paid,
                PaymentMethod = premiumDto.PaymentMethod,
                TransactionReference = "TXN" + DateTimeExtensions.NowIst().ToString("yyyyMMddHHmmss"),
                CommissionAmount = await CalculateCommissionAsync(premiumDto.PolicyId, premiumDto.PremiumAmount)
            };

            Console.WriteLine($"[PREMIUM] Creating premium record...");
            await _premiumRepository.CreateAsync(premium);
            Console.WriteLine($"[PREMIUM] Premium record created successfully");

            Console.WriteLine($"[PREMIUM] Updating policy {premiumDto.PolicyId}...");
            if (policy != null)
            {
                Console.WriteLine($"[PREMIUM] Policy found, updating total premium paid");
                policy.TotalPremiumPaid += premiumDto.PremiumAmount;
                await _policyRepository.UpdateAsync(policy);
                
                if (policy.AgentId.HasValue)
                {
                    Console.WriteLine($"[PREMIUM] Updating agent commission for agent {policy.AgentId.Value}");
                    var agent = await _userRepository.GetByIdAsync(policy.AgentId.Value);
                    if (agent != null)
                    {
                        agent.TotalCommissionEarned += premium.CommissionAmount;
                        await _userRepository.UpdateAsync(agent);
                    }
                }
            }
            else
            {
                Console.WriteLine($"[PREMIUM] ERROR: Policy {premiumDto.PolicyId} not found!");
            }

            Console.WriteLine($"[PREMIUM] Creating investment transaction for Fund {premiumDto.FundId}...");
            try
            {
                var investmentResult = await _transactionService.ProcessInvestmentAsync(premiumDto.PolicyId, premiumDto.PremiumAmount, premiumDto.FundId);
                Console.WriteLine($"[PREMIUM] Investment transaction created successfully: {investmentResult?.TransactionId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PREMIUM] ERROR: Investment failed - {ex.Message}");
                throw new Exception($"Premium payment processed but investment failed: {ex.Message}");
            }

            Console.WriteLine($"[PREMIUM] Premium payment completed successfully");
            return true;
        }

        private async Task ValidatePaymentFrequencyAsync(int policyId, PremiumFrequency frequency)
        {
            var existingPayments = await _premiumRepository.GetByPolicyIdAsync(policyId);
            var now = DateTimeExtensions.NowIst();
            
            var recentPayment = frequency switch
            {
                PremiumFrequency.Monthly => existingPayments.FirstOrDefault(p => 
                    p.Status == PremiumStatus.Paid && 
                    p.PaidDate.HasValue &&
                    p.PaidDate.Value.Month == now.Month && 
                    p.PaidDate.Value.Year == now.Year),
                    
                PremiumFrequency.Quarterly => existingPayments.FirstOrDefault(p => 
                    p.Status == PremiumStatus.Paid && 
                    p.PaidDate.HasValue &&
                    GetQuarter(p.PaidDate.Value) == GetQuarter(now) && 
                    p.PaidDate.Value.Year == now.Year),
                    
                PremiumFrequency.HalfYearly => existingPayments.FirstOrDefault(p => 
                    p.Status == PremiumStatus.Paid && 
                    p.PaidDate.HasValue &&
                    GetHalfYear(p.PaidDate.Value) == GetHalfYear(now) && 
                    p.PaidDate.Value.Year == now.Year),
                    
                PremiumFrequency.Yearly => existingPayments.FirstOrDefault(p => 
                    p.Status == PremiumStatus.Paid && 
                    p.PaidDate.HasValue &&
                    p.PaidDate.Value.Year == now.Year),
                    
                _ => null
            };
            
            if (recentPayment != null)
            {
                var frequencyText = frequency switch
                {
                    PremiumFrequency.Monthly => "month",
                    PremiumFrequency.Quarterly => "quarter",
                    PremiumFrequency.HalfYearly => "half-year",
                    PremiumFrequency.Yearly => "year",
                    _ => "period"
                };
                throw new Exception($"Premium already paid for this {frequencyText}. Last payment on {recentPayment.PaidDate:dd-MM-yyyy}");
            }
        }
        
        private int GetQuarter(DateTime date)
        {
            return (date.Month - 1) / 3 + 1;
        }
        
        private int GetHalfYear(DateTime date)
        {
            return date.Month <= 6 ? 1 : 2;
        }

        public async Task<decimal> CalculateCommissionAsync(int policyId, decimal premiumAmount)
        {
            return premiumAmount * 0.05m;
        }

        public async Task<IEnumerable<PremiumDto>> GetOverduePremumsAsync()
        {
            var overduePremiums = await _premiumRepository.GetOverdueAsync();
            return overduePremiums.Select(p => new PremiumDto
            {
                PremiumId = p.PremiumId,
                PolicyId = p.PolicyId,
                PremiumAmount = p.PremiumAmount,
                DueDate = p.DueDate,
                Status = p.Status,
                PolicyNumber = p.Policy?.PolicyNumber,
                CustomerName = p.Policy?.User?.FirstName + " " + p.Policy?.User?.LastName
            });
        }

        public async Task<bool> SendReminderAsync(int policyId)
        {
            var policy = await _policyRepository.GetByIdAsync(policyId);
            if (policy?.User != null)
            {
                await _emailService.SendPremiumReminderEmailAsync(
                    policy.User.Email,
                    $"{policy.User.FirstName} {policy.User.LastName}",
                    policy.PolicyNumber,
                    policy.PremiumAmount,
                    DateTime.Now.AddDays(30)
                );
            }
            return true;
        }

        public async Task<bool> GeneratePremiumScheduleAsync(int policyId)
        {
            var policy = await _policyRepository.GetByIdAsync(policyId);
            if (policy == null) return false;

            var startDate = policy.PolicyStartDate;
            var endDate = policy.PolicyMaturityDate;
            var frequency = policy.PremiumFrequency;
            var amount = policy.PremiumAmount;

            var premiums = new List<Premium>();
            var currentDate = startDate;

            while (currentDate <= endDate)
            {
                premiums.Add(new Premium
                {
                    PolicyId = policyId,
                    PremiumAmount = amount,
                    DueDate = currentDate,
                    Status = PremiumStatus.Pending,
                    CommissionAmount = await CalculateCommissionAsync(policyId, amount)
                });

                currentDate = frequency switch
                {
                    PremiumFrequency.Monthly => currentDate.AddMonths(1),
                    PremiumFrequency.Quarterly => currentDate.AddMonths(3),
                    PremiumFrequency.HalfYearly => currentDate.AddMonths(6),
                    PremiumFrequency.Yearly => currentDate.AddYears(1),
                    _ => currentDate.AddYears(1)
                };
            }

            foreach (var premium in premiums)
            {
                await _premiumRepository.CreateAsync(premium);
            }

            return true;
        }

        public async Task<IEnumerable<PremiumDto>> GetPremiumScheduleAsync(int policyId)
        {
            var premiums = await _premiumRepository.GetByPolicyIdAsync(policyId);
            return premiums.Select(p => new PremiumDto
            {
                PremiumId = p.PremiumId,
                PolicyId = p.PolicyId,
                PremiumAmount = p.PremiumAmount,
                DueDate = p.DueDate,
                PaidDate = p.PaidDate,
                Status = p.Status,
                PaymentMethod = p.PaymentMethod,
                TransactionReference = p.TransactionReference
            });
        }

        public async Task<bool> ProcessOverduePoliciesAsync()
        {
            var overduePremiums = await _premiumRepository.GetOverdueAsync();
            var gracePeriodDays = 30;

            foreach (var premium in overduePremiums)
            {
                var policy = await _policyRepository.GetByIdAsync(premium.PolicyId);
                if (policy == null) continue;

                var daysPastDue = (DateTime.Now - premium.DueDate).Days;

                if (daysPastDue > gracePeriodDays && policy.PolicyStatus == PolicyStatus.Active)
                {
                    policy.PolicyStatus = PolicyStatus.Lapsed;
                    await _policyRepository.UpdateAsync(policy);

                    if (policy.User != null)
                    {
                        await _emailService.SendPolicyLapseNotificationAsync(
                            policy.User.Email,
                            $"{policy.User.FirstName} {policy.User.LastName}",
                            policy.PolicyNumber,
                            premium.PremiumAmount,
                            premium.DueDate
                        );
                    }
                }
                else if (daysPastDue >= 7 && daysPastDue <= gracePeriodDays)
                {
                    await SendReminderAsync(premium.PolicyId);
                }
            }

            return true;
        }
    }
}
