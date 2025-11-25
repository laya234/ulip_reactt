using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ULIP_proj.Enums;

namespace ULIP_proj.Models
{
    public class Premium
    {
        [Key]
        public int PremiumId { get; set; }

        [Required]
        public int PolicyId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PremiumAmount { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public DateTime? PaidDate { get; set; }

        [Required]
        public PremiumStatus Status { get; set; } = PremiumStatus.Pending;

        public PaymentMethod? PaymentMethod { get; set; }

        [StringLength(100)]
        public string? TransactionReference { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        public decimal CommissionAmount { get; set; } = 0;
        public bool CommissionPaid { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        [ForeignKey("PolicyId")]
        public Policy Policy { get; set; }
    }
}
