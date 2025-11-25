using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ULIP_proj.Enums;

namespace ULIP_proj.Models
{
    public class PolicyDocument
    {
        [Key]
        public int DocumentId { get; set; }

        [Required]
        public int PolicyId { get; set; }

        [Required]
        public DocumentType DocumentType { get; set; }

        [Required]
        [StringLength(500)]
        public string FilePath { get; set; }

        [Required]
        public DocumentStatus Status { get; set; } = DocumentStatus.Uploaded;

        public int? VerifiedBy { get; set; }
        public DateTime? VerifiedAt { get; set; }

        [StringLength(500)]
        public string? RejectionReason { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("PolicyId")]
        public Policy Policy { get; set; }

        [ForeignKey("VerifiedBy")]
        public User? VerifiedByUser { get; set; }
    }
}
