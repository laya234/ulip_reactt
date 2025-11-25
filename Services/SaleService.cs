using ULIP_proj.DTOs;
using ULIP_proj.Interfaces;
using ULIP_proj.Models;
using ULIP_proj.Enums;

namespace ULIP_proj.Services
{
    public class SaleService : ISaleService
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IPolicyService _policyService;

        public SaleService(ISaleRepository saleRepository, IPolicyService policyService)
        {
            _saleRepository = saleRepository;
            _policyService = policyService;
        }

        public async Task<SaleDto> CreateLeadAsync(CreateSaleDto saleDto, int agentId)
        {
            var sale = new Sale
            {
                AgentId = agentId,
                CustomerName = saleDto.CustomerName,
                CustomerPhone = saleDto.CustomerPhone,
                Status = SaleStatus.Lead,
                QuotedAmount = saleDto.QuotedAmount,
                Notes = saleDto.Notes
            };

            var createdSale = await _saleRepository.CreateAsync(sale);
            return MapToSaleDto(createdSale);
        }

        public async Task<bool> ConvertLeadToPolicyAsync(int saleId, CreatePolicyDto policyDto, int agentId)
        {
            var sales = await _saleRepository.GetByAgentIdAsync(agentId);
            var sale = sales.FirstOrDefault(s => s.Id == saleId);
            
            if (sale == null) return false;

            var policy = await _policyService.CreatePolicyAsync(policyDto, agentId);
            sale.Status = SaleStatus.Converted;
            sale.PolicyId = policy.PolicyId;
            await _saleRepository.UpdateAsync(sale);

            return true;
        }

        public async Task<IEnumerable<SaleDto>> GetMyPipelineAsync(int agentId)
        {
            var sales = await _saleRepository.GetByAgentIdAsync(agentId);
            return sales.Select(MapToSaleDto);
        }

        public async Task<decimal> GetMyConversionRateAsync(int agentId)
        {
            var sales = await _saleRepository.GetByAgentIdAsync(agentId);
            
            var totalLeads = sales.Count();
            var convertedLeads = sales.Count(s => s.Status == SaleStatus.Converted);
            
            return totalLeads > 0 ? (decimal)convertedLeads / totalLeads * 100 : 0;
        }



        private SaleDto MapToSaleDto(Sale sale)
        {
            return new SaleDto
            {
                Id = sale.Id,
                AgentId = sale.AgentId,
                CustomerName = sale.CustomerName,
                CustomerPhone = sale.CustomerPhone,
                PolicyId = sale.PolicyId,
                Status = sale.Status,
                QuotedAmount = sale.QuotedAmount,
                Notes = sale.Notes,
                CreatedDate = sale.CreatedDate
            };
        }
    }
}
