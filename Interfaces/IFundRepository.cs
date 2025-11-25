using ULIP_proj.Models;

namespace ULIP_proj.Interfaces
{
    public interface IFundRepository
    {
        Task<IEnumerable<Fund>> GetAllAsync();
        Task<Fund> GetByIdAsync(int fundId);
        Task<Fund> CreateAsync(Fund fund);
        Task<Fund> UpdateNAVAsync(int fundId, decimal newNAV);
    }
}
