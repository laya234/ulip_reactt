using ULIP_proj.Models;

namespace ULIP_proj.Interfaces
{
    public interface ISaleRepository
    {
        Task<Sale> CreateAsync(Sale sale);
        Task<Sale> UpdateAsync(Sale sale);
        Task<IEnumerable<Sale>> GetByAgentIdAsync(int agentId);
        Task<IEnumerable<Sale>> GetLeadsAsync();
    }
}
