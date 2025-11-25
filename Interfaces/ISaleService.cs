using ULIP_proj.DTOs;

namespace ULIP_proj.Interfaces
{
    public interface ISaleService
    {
        Task<SaleDto> CreateLeadAsync(CreateSaleDto saleDto, int agentId);
        Task<bool> ConvertLeadToPolicyAsync(int saleId, CreatePolicyDto policyDto, int agentId);
        Task<IEnumerable<SaleDto>> GetMyPipelineAsync(int agentId);
        Task<decimal> GetMyConversionRateAsync(int agentId);
    }
}
