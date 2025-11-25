using Microsoft.EntityFrameworkCore;
using ULIP_proj.Data;
using ULIP_proj.Interfaces;
using ULIP_proj.Models;
using ULIP_proj.Enums;

namespace ULIP_proj.Repositories
{
    public class PolicyDocumentRepository : IPolicyDocumentRepository
    {
        private readonly ULIPDbContext _context;

        public PolicyDocumentRepository(ULIPDbContext context)
        {
            _context = context;
        }

        public async Task<PolicyDocument> CreateAsync(PolicyDocument document)
        {
            _context.PolicyDocuments.Add(document);
            await _context.SaveChangesAsync();
            return document;
        }

        public async Task<IEnumerable<PolicyDocument>> GetByPolicyIdAsync(int policyId)
        {
            return await _context.PolicyDocuments
                .Where(d => d.PolicyId == policyId)
                .Include(d => d.Policy)
                .ToListAsync();
        }

        public async Task<IEnumerable<PolicyDocument>> GetUploadedDocumentsAsync()
        {
            var allDocs = await _context.PolicyDocuments
                .Include(d => d.Policy)
                .ThenInclude(p => p.User)
                .ToListAsync();
            
            Console.WriteLine($"Total documents in DB: {allDocs.Count}");
            foreach(var doc in allDocs)
            {
                Console.WriteLine($"Doc ID: {doc.DocumentId}, Status: {doc.Status}, PolicyId: {doc.PolicyId}");
            }
            
            return allDocs.Where(d => d.Status == DocumentStatus.Uploaded).ToList();
        }

        public async Task<PolicyDocument?> GetByPolicyIdAndTypeAsync(int policyId, string documentType)
        {
            var docType = Enum.Parse<DocumentType>(documentType);
            return await _context.PolicyDocuments
                .FirstOrDefaultAsync(d => d.PolicyId == policyId && d.DocumentType == docType);
        }

        public async Task<PolicyDocument> UpdateAsync(PolicyDocument document)
        {
            _context.PolicyDocuments.Update(document);
            await _context.SaveChangesAsync();
            return document;
        }
    }
}
