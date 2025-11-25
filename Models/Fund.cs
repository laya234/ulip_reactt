using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ULIP_proj.Enums;

namespace ULIP_proj.Models
{
    public class Fund
    {
        [Key]
        public int FundId { get; set; }

        [Required]
        [StringLength(200)]
        public string FundName { get; set; }

        [Required]
        public FundType FundType { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal CurrentNAV { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal ExpenseRatio { get; set; }

        [Required]
        public RiskLevel RiskLevel { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
