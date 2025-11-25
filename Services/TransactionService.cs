using Microsoft.Extensions.Logging;
using ULIP_proj.DTOs;
using ULIP_proj.Interfaces;
using ULIP_proj.Models;
using ULIP_proj.Enums;
using ULIP_proj.Extensions;

namespace ULIP_proj.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IFundRepository _fundRepository;
        private readonly IPolicyRepository _policyRepository;
        private readonly IApprovalRepository _approvalRepository;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(ITransactionRepository transactionRepository, IFundRepository fundRepository, IPolicyRepository policyRepository, IApprovalRepository approvalRepository, IEmailService emailService, IUserRepository userRepository, ILogger<TransactionService> logger)
        {
            _transactionRepository = transactionRepository;
            _fundRepository = fundRepository;
            _policyRepository = policyRepository;
            _approvalRepository = approvalRepository;
            _emailService = emailService;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<TransactionDto> ProcessInvestmentAsync(int policyId, decimal amount, int fundId)
        {
            _logger.LogInformation("Processing investment: Policy {PolicyId}, Amount {Amount}, Fund {FundId}", policyId, amount, fundId);
            var fund = await _fundRepository.GetByIdAsync(fundId);
            if (fund == null) return null;
            
            var units = amount / fund.CurrentNAV;

            var transaction = new Transaction
            {
                PolicyId = policyId,
                FundId = fundId,
                TransactionType = TransactionType.Purchase,
                Amount = amount,
                Units = units,
                NAV = fund.CurrentNAV,
                TransactionDate = DateTimeExtensions.NowIst(),
                Description = $"Premium investment of ₹{amount} in {fund.FundName}",
                RequiresApproval = amount > 100000,
                IsApproved = amount <= 100000
            };

            var createdTransaction = await _transactionRepository.CreateAsync(transaction);
            _logger.LogInformation("Investment processed: Transaction {TransactionId}, Units {Units}", createdTransaction.TransactionId, units);
            
            if (amount > 100000)
            {
                await RequestHighValueApprovalAsync(createdTransaction.TransactionId);
            }
            
            var currentValue = await GetPolicyCurrentValueAsync(policyId);
            var policy = await _policyRepository.GetByIdAsync(policyId);
            if (policy != null)
            {
                policy.CurrentValue = currentValue;
                await _policyRepository.UpdateAsync(policy);
            }

            return MapToTransactionDto(createdTransaction);
        }

        public async Task<bool> ProcessFundSwitchAsync(int policyId, int fromFundId, int toFundId, decimal amount)
        {
            _logger.LogInformation("Processing fund switch: Policy {PolicyId}, From Fund {FromFundId} to Fund {ToFundId}, Amount {Amount}", policyId, fromFundId, toFundId, amount);
            
            var fromFund = await _fundRepository.GetByIdAsync(fromFundId);
            if (fromFund == null)
            {
                _logger.LogWarning("From Fund {FromFundId} not found", fromFundId);
                throw new ArgumentException($"From Fund {fromFundId} not found");
            }
            var redeemUnits = amount / fromFund.CurrentNAV;

            var redemptionTransaction = new Transaction
            {
                PolicyId = policyId,
                FundId = fromFundId,
                TransactionType = TransactionType.Redemption,
                Amount = amount,
                Units = redeemUnits,
                NAV = fromFund.CurrentNAV,
                TransactionDate = DateTimeExtensions.NowIst(),
                Description = $"Fund switch redemption from {fromFund.FundName}",
                RequiresApproval = false,
                IsApproved = true
            };

            var toFund = await _fundRepository.GetByIdAsync(toFundId);
            if (toFund == null)
            {
                _logger.LogWarning("To Fund {ToFundId} not found", toFundId);
                throw new ArgumentException($"To Fund {toFundId} not found");
            }
            var purchaseUnits = amount / toFund.CurrentNAV;

            var purchaseTransaction = new Transaction
            {
                PolicyId = policyId,
                FundId = toFundId,
                TransactionType = TransactionType.Purchase,
                Amount = amount,
                Units = purchaseUnits,
                NAV = toFund.CurrentNAV,
                TransactionDate = DateTimeExtensions.NowIst(),
                Description = $"Fund switch purchase to {toFund.FundName}",
                RequiresApproval = false,
                IsApproved = true
            };

            var redemptionResult = await _transactionRepository.CreateAsync(redemptionTransaction);
            var purchaseResult = await _transactionRepository.CreateAsync(purchaseTransaction);
            
            var currentValue = await GetPolicyCurrentValueAsync(policyId);
            var policy = await _policyRepository.GetByIdAsync(policyId);
            if (policy != null)
            {
                policy.CurrentValue = currentValue;
                await _policyRepository.UpdateAsync(policy);
            }
            
            _logger.LogInformation("Fund switch transactions created: Redemption {RedemptionId}, Purchase {PurchaseId}", 
                redemptionResult.TransactionId, purchaseResult.TransactionId);
            _logger.LogInformation("Fund switch completed for policy {PolicyId}, updated current value to {CurrentValue}", policyId, currentValue);
            
            if (policy != null)
            {
                var customer = await _userRepository.GetByIdAsync(policy.UserId);
                if (customer != null)
                {
                    await _emailService.SendFundSwitchConfirmationEmailAsync(
                        customer.Email,
                        $"{customer.FirstName} {customer.LastName}",
                        fromFund.FundName,
                        toFund.FundName,
                        amount);
                }
            }

            return true;
        }

        public async Task<bool> RequestHighValueApprovalAsync(int transactionId)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);
            if (transaction == null || transaction.Amount <= 100000) return false;

            var policy = await _policyRepository.GetByIdAsync(transaction.PolicyId);
            if (policy == null) return false;

            var approval = new Approval
            {
                RequestId = transactionId,
                RequestType = ApprovalType.HighValueTransaction,
                RequestedBy = policy.UserId,
                Status = ApprovalStatus.Pending,
                Amount = transaction.Amount,
                RequestedAt = DateTimeExtensions.NowIst()
            };

            await _approvalRepository.CreateAsync(approval);
            await _emailService.SendApprovalNotificationEmailAsync("manager@ulip.com", "Manager", "High Value Transaction", false, $"Transaction of ₹{transaction.Amount} requires approval");
            
            return true;
        }

        public async Task<decimal> GetPolicyCurrentValueAsync(int policyId)
        {
            try
            {
                var transactions = await _transactionRepository.GetByPolicyIdAsync(policyId);
                
                if (transactions == null || !transactions.Any())
                    return 0;
                
                decimal totalValue = 0;
                var fundGroups = transactions.GroupBy(t => t.FundId);

                foreach (var group in fundGroups)
                {
                    var fund = await _fundRepository.GetByIdAsync(group.Key);
                    if (fund != null)
                    {
                        var totalUnits = group.Where(t => t.TransactionType == TransactionType.Purchase).Sum(t => t.Units) -
                                        group.Where(t => t.TransactionType == TransactionType.Redemption).Sum(t => t.Units);
                        
                        totalValue += totalUnits * fund.CurrentNAV;
                    }
                }

                return totalValue;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<IEnumerable<TransactionDto>> GetTransactionsByPolicyAsync(int policyId)
        {
            _logger.LogInformation("Getting transactions for policy {PolicyId}", policyId);
            var transactions = await _transactionRepository.GetByPolicyIdAsync(policyId);
            _logger.LogInformation("Found {Count} transactions for policy {PolicyId}", transactions?.Count() ?? 0, policyId);
            
            if (transactions != null && transactions.Any())
            {
                foreach (var t in transactions)
                {
                    _logger.LogInformation("Transaction: ID={TransactionId}, Type={Type}, Amount={Amount}, Date={Date}", 
                        t.TransactionId, t.TransactionType, t.Amount, t.TransactionDate);
                }
            }
            
            return transactions?.Select(MapToTransactionDto) ?? new List<TransactionDto>();
        }



        public async Task<PortfolioValueDto> CalculatePortfolioValueAsync(int policyId)
        {
            var transactions = await _transactionRepository.GetByPolicyIdAsync(policyId);
            var fundHoldings = new List<FundHoldingDto>();
            
            if (transactions == null || !transactions.Any())
            {
                return new PortfolioValueDto
                {
                    FundHoldings = fundHoldings,
                    TotalInvested = 0,
                    TotalCurrentValue = 0,
                    TotalGainLoss = 0,
                    ReturnPercentage = 0
                };
            }
            
            var fundGroups = transactions.GroupBy(t => t.FundId);
            
            foreach (var group in fundGroups)
            {
                var fund = await _fundRepository.GetByIdAsync(group.Key);
                if (fund == null) continue;
                
                var purchases = group.Where(t => t.TransactionType == TransactionType.Purchase);
                var redemptions = group.Where(t => t.TransactionType == TransactionType.Redemption);
                
                var totalUnits = purchases.Sum(t => t.Units) - redemptions.Sum(t => t.Units);
                var totalInvested = purchases.Sum(t => t.Amount) - redemptions.Sum(t => t.Amount);
                
                if (totalUnits <= 0) continue;
                
                var avgPurchaseNAV = purchases.Any() ? purchases.Sum(t => t.Amount) / purchases.Sum(t => t.Units) : 0;
                var currentValue = totalUnits * fund.CurrentNAV;
                var gainLoss = currentValue - totalInvested;
                var returnPercentage = totalInvested > 0 ? (gainLoss / totalInvested) * 100 : 0;
                
                _logger.LogInformation($"Fund {fund.FundName}: Units={totalUnits}, Invested={totalInvested}, AvgNAV={avgPurchaseNAV}, CurrentNAV={fund.CurrentNAV}, CurrentValue={currentValue}, Return={returnPercentage}%");
                
                fundHoldings.Add(new FundHoldingDto
                {
                    FundId = fund.FundId,
                    FundName = fund.FundName,
                    TotalUnits = totalUnits,
                    TotalInvested = totalInvested,
                    CurrentNAV = fund.CurrentNAV,
                    CurrentValue = currentValue,
                    GainLoss = gainLoss,
                    ReturnPercentage = returnPercentage
                });
            }
            
            var portfolioTotalInvested = fundHoldings.Sum(f => f.TotalInvested);
            var portfolioTotalCurrentValue = fundHoldings.Sum(f => f.CurrentValue);
            var portfolioTotalGainLoss = portfolioTotalCurrentValue - portfolioTotalInvested;
            var portfolioReturnPercentage = portfolioTotalInvested > 0 ? (portfolioTotalGainLoss / portfolioTotalInvested) * 100 : 0;
            
            return new PortfolioValueDto
            {
                FundHoldings = fundHoldings,
                TotalInvested = portfolioTotalInvested,
                TotalCurrentValue = portfolioTotalCurrentValue,
                TotalGainLoss = portfolioTotalGainLoss,
                ReturnPercentage = portfolioReturnPercentage
            };
        }

        private TransactionDto MapToTransactionDto(Transaction transaction)
        {
            return new TransactionDto
            {
                TransactionId = transaction.TransactionId,
                PolicyId = transaction.PolicyId,
                FundId = transaction.FundId,
                TransactionType = transaction.TransactionType,
                Amount = transaction.Amount,
                Units = transaction.Units,
                NAV = transaction.NAV,
                TransactionDate = transaction.TransactionDate,
                Description = transaction.Description ?? "",
                FundName = transaction.Fund?.FundName ?? $"Fund {transaction.FundId}"
            };
        }
    }
}
