using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ULIP_proj.Enums;

namespace ULIP_proj.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        [Required]
        public int PolicyId { get; set; }

        [Required]
        public int FundId { get; set; }

        [Required]
        public TransactionType TransactionType { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal Units { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal NAV { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        [StringLength(200)]
        public string? Description { get; set; }


        public bool RequiresApproval { get; set; } = false;
        public bool IsApproved { get; set; } = false;
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("PolicyId")]
        public Policy Policy { get; set; }

        [ForeignKey("FundId")]
        public Fund Fund { get; set; }
    }
}
