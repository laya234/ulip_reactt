using ULIP_proj.DTOs;

namespace ULIP_proj.Interfaces
{
    public interface IFundService
    {
        Task<IEnumerable<FundDto>> GetAvailableFundsAsync();
        Task<FundDto> CreateFundAsync(CreateFundDto fundDto);
        Task<bool> UpdateDailyNAVAsync(int fundId, decimal newNAV);
        Task<FundDetailsDto> GetFundDetailsAsync(int fundId);
    }
}
