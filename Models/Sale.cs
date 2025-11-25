using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ULIP_proj.Enums;

namespace ULIP_proj.Models
{
    public class Sale
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AgentId { get; set; }

        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; }

        [Required]
        [StringLength(15)]
        public string CustomerPhone { get; set; }

        public int? PolicyId { get; set; }

        [Required]
        public SaleStatus Status { get; set; } = SaleStatus.Lead;

        [Column(TypeName = "decimal(18,2)")]
        public decimal QuotedAmount { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [ForeignKey("AgentId")]
        public User Agent { get; set; }

        [ForeignKey("PolicyId")]
        public Policy? Policy { get; set; }
    }
}
