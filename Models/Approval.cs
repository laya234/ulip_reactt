using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ULIP_proj.Enums;

namespace ULIP_proj.Models
{
    public class Approval
    {
        [Key]
        public int ApprovalId { get; set; }

        [Required]
        public int RequestId { get; set; }

        [Required]
        public ApprovalType RequestType { get; set; }

        [Required]
        public int RequestedBy { get; set; }

        public int? ApprovedBy { get; set; }

        [Required]
        public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Amount { get; set; }

        [StringLength(500)]
        public string RequestReason { get; set; }

        [StringLength(500)]
        public string? ApprovalComments { get; set; }

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ApprovedAt { get; set; }

        [ForeignKey("RequestedBy")]
        public User RequestedByUser { get; set; }

        [ForeignKey("ApprovedBy")]
        public User ApprovedByUser { get; set; }
    }
}
