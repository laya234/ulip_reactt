using System.ComponentModel.DataAnnotations;
using ULIP_proj.Enums;

namespace ULIP_proj.DTOs
{
    public class CreatePolicyProposalDto
    {
        [Required]
        [StringLength(100)]
        public string PolicyName { get; set; }

        [Required]
        public decimal SumAssured { get; set; }

        [Required]
        public decimal PremiumAmount { get; set; }

        [Required]
        public PremiumFrequency PremiumFrequency { get; set; }

        [Required]
        public DateTime PolicyStartDate { get; set; }

        [Required]
        public DateTime PolicyMaturityDate { get; set; }
    }

    public class PolicyAcceptanceDto
    {
        [Required]
        public bool Accepted { get; set; }
        
        public string? RejectionReason { get; set; }
        public bool RequireDocuments { get; set; } = false;
    }

    public class PolicyProposalDto
    {
        public int PolicyId { get; set; }
        public string PolicyNumber { get; set; }
        public string PolicyName { get; set; }
        public decimal SumAssured { get; set; }
        public decimal PremiumAmount { get; set; }
        public PremiumFrequency PremiumFrequency { get; set; }
        public DateTime PolicyStartDate { get; set; }
        public DateTime PolicyMaturityDate { get; set; }
        public PolicyStatus PolicyStatus { get; set; }
        public DateTime? ProposedDate { get; set; }
        public string AgentName { get; set; }
    }
}
