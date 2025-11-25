using ULIP_proj.DTOs;
using ULIP_proj.Interfaces;
using ULIP_proj.Models;
using ULIP_proj.Enums;

namespace ULIP_proj.Services
{
    public class FundService : IFundService
    {
        private readonly IFundRepository _fundRepository;

        public FundService(IFundRepository fundRepository)
        {
            _fundRepository = fundRepository;
        }

        public async Task<IEnumerable<FundDto>> GetAvailableFundsAsync()
        {
            var funds = await _fundRepository.GetAllAsync();
            return funds.Select(f => new FundDto
            {
                FundId = f.FundId,
                FundName = f.FundName,
                FundType = f.FundType,
                CurrentNAV = f.CurrentNAV,
                RiskLevel = f.RiskLevel,
                ExpenseRatio = f.ExpenseRatio
            });
        }

        public async Task<bool> UpdateDailyNAVAsync(int fundId, decimal newNAV)
        {
            var fund = await _fundRepository.UpdateNAVAsync(fundId, newNAV);
            return fund != null;
        }

        public async Task<FundDto> CreateFundAsync(CreateFundDto fundDto)
        {
            var fund = new Fund
            {
                FundName = fundDto.FundName,
                FundType = fundDto.FundType,
                CurrentNAV = fundDto.CurrentNAV,
                ExpenseRatio = fundDto.ExpenseRatio,
                RiskLevel = fundDto.RiskLevel,
                Description = fundDto.Description
            };

            var createdFund = await _fundRepository.CreateAsync(fund);
            return new FundDto
            {
                FundId = createdFund.FundId,
                FundName = createdFund.FundName,
                FundType = createdFund.FundType,
                CurrentNAV = createdFund.CurrentNAV,
                RiskLevel = createdFund.RiskLevel,
                ExpenseRatio = createdFund.ExpenseRatio,
                Description = createdFund.Description,
                UpdatedAt = createdFund.UpdatedAt
            };
        }

        public Task<FundDetailsDto> GetFundDetailsAsync(int fundId)
        {
            var fund = _fundRepository.GetByIdAsync(fundId).Result;
            if (fund == null) return Task.FromResult<FundDetailsDto>(null);
            
            var benefits = GetFundBenefits(fund.FundType, fund.RiskLevel);
            var suitableFor = GetSuitableFor(fund.FundType, fund.RiskLevel);
            var returns = CalculateHistoricalReturns(fund.FundType);
            
            return Task.FromResult(new FundDetailsDto
            {
                FundId = fund.FundId,
                FundName = fund.FundName,
                FundType = fund.FundType.ToString(),
                CurrentNAV = fund.CurrentNAV,
                ExpenseRatio = fund.ExpenseRatio,
                RiskLevel = fund.RiskLevel.ToString(),
                Description = fund.Description ?? GetDefaultDescription(fund.FundType),
                OneYearReturn = returns.OneYear,
                ThreeYearReturn = returns.ThreeYear,
                FiveYearReturn = returns.FiveYear,
                Benefits = benefits,
                SuitableFor = suitableFor
            });
        }

        private string GetFundBenefits(FundType fundType, RiskLevel riskLevel)
        {
            return fundType switch
            {
                FundType.Equity => "• High growth potential\n• Long-term wealth creation\n• Tax benefits under Section 80C\n• Professional fund management\n• Diversified portfolio",
                FundType.Debt => "• Stable returns\n• Lower risk compared to equity\n• Regular income generation\n• Capital preservation\n• Suitable for conservative investors",
                FundType.Balanced => "• Balanced risk-return profile\n• Diversification across asset classes\n• Moderate growth potential\n• Reduced volatility\n• Suitable for medium-term goals",
                FundType.Hybrid => "• Balanced asset allocation\n• Moderate risk-return\n• Diversified portfolio\n• Flexible investment\n• Suitable for varied goals",
                _ => "• Professional management\n• Diversified investment\n• Tax benefits\n• Flexible investment options"
            };
        }

        private string GetSuitableFor(FundType fundType, RiskLevel riskLevel)
        {
            return fundType switch
            {
                FundType.Equity => "Investors with high risk appetite, long-term investment horizon (7+ years), and seeking wealth creation",
                FundType.Debt => "Conservative investors seeking stable returns, short to medium-term goals, and capital preservation",
                FundType.Balanced => "Moderate risk-takers, medium-term investment horizon (3-5 years), balanced growth seekers",
                FundType.Hybrid => "Investors seeking diversification, moderate risk appetite, flexible investment horizon",
                _ => "All types of investors based on their risk profile and investment goals"
            };
        }

        private string GetDefaultDescription(FundType fundType)
        {
            return fundType switch
            {
                FundType.Equity => "This equity fund invests primarily in stocks and equity-related instruments, offering high growth potential with higher risk.",
                FundType.Debt => "This debt fund invests in fixed-income securities like bonds and government securities, providing stable returns with lower risk.",
                FundType.Balanced => "This balanced fund maintains a mix of equity and debt instruments, offering moderate growth with balanced risk.",
                FundType.Hybrid => "This hybrid fund combines multiple asset classes, offering diversification with moderate risk and return potential.",
                _ => "A professionally managed investment fund designed to meet your financial goals."
            };
        }

        private (decimal OneYear, decimal ThreeYear, decimal FiveYear) CalculateHistoricalReturns(FundType fundType)
        {
            return fundType switch
            {
                FundType.Equity => (12.5m, 15.8m, 18.2m),
                FundType.Debt => (6.5m, 7.2m, 7.8m),
                FundType.Balanced => (9.5m, 11.2m, 12.5m),
                FundType.Hybrid => (8.5m, 10.2m, 11.5m),
                _ => (8.0m, 9.0m, 10.0m)
            };
        }

    }
}
