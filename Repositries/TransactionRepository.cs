using Microsoft.EntityFrameworkCore;
using ULIP_proj.Data;
using ULIP_proj.Interfaces;
using ULIP_proj.Models;

namespace ULIP_proj.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ULIPDbContext _context;

        public TransactionRepository(ULIPDbContext context)
        {
            _context = context;
        }

        public async Task<Transaction> CreateAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<Transaction> UpdateAsync(Transaction transaction)
        {
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<Transaction?> GetByIdAsync(int transactionId)
        {
            return await _context.Transactions
                .Include(t => t.Policy)
                .Include(t => t.Fund)
                .FirstOrDefaultAsync(t => t.TransactionId == transactionId);
        }

        public async Task<IEnumerable<Transaction>> GetByPolicyIdAsync(int policyId)
        {
            return await _context.Transactions
                .Where(t => t.PolicyId == policyId)
                .Include(t => t.Fund)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetPendingApprovalsAsync()
        {
            return await _context.Transactions
                .Where(t => t.RequiresApproval == true && t.IsApproved == false)
                .Include(t => t.Policy)
                .ThenInclude(p => p.User)
                .Include(t => t.Fund)
                .ToListAsync();
        }
    }
}
