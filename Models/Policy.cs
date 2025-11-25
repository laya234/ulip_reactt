using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ULIP_proj.Enums;

namespace ULIP_proj.Models
{
    public class Policy
    {
        [Key]
        public int PolicyId { get; set; }

        [Required]
        [StringLength(50)]
        public string PolicyNumber { get; set; }

        [Required]
        public int UserId { get; set; }

        public int? AgentId { get; set; }

        [Required]
        [StringLength(100)]
        public string PolicyName { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SumAssured { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PremiumAmount { get; set; }

        [Required]
        public PremiumFrequency PremiumFrequency { get; set; }

        [Required]
        public DateTime PolicyStartDate { get; set; }

        [Required]
        public DateTime PolicyMaturityDate { get; set; }

        [Required]
        public PolicyStatus PolicyStatus { get; set; } = PolicyStatus.Proposed;


        public DateTime? ProposedDate { get; set; }
        public DateTime? AcceptedDate { get; set; }
        public DateTime? RejectedDate { get; set; }
        [StringLength(500)]
        public string? RejectionReason { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentValue { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPremiumPaid { get; set; }


        public bool SurrenderRequested { get; set; } = false;
        public DateTime? SurrenderRequestDate { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SurrenderValue { get; set; }
        public SurrenderStatus? SurrenderStatus { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        public decimal CommissionPaid { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool KYCVerified { get; set; } = false;
        public bool MedicalVerified { get; set; } = false;
        public DateTime? KYCVerifiedAt { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }


        public ICollection<Premium> Premiums { get; set; } = new List<Premium>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public ICollection<PolicyDocument> Documents { get; set; } = new List<PolicyDocument>();
    }
}
