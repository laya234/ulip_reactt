using System.ComponentModel.DataAnnotations;
using ULIP_proj.Enums;

namespace ULIP_proj.DTOs
{
    public class CreateFundDto
    {
        [Required]
        public string FundName { get; set; }
        [Required]
        public FundType FundType { get; set; }
        [Required]
        public decimal CurrentNAV { get; set; }
        [Required]
        public decimal ExpenseRatio { get; set; }
        [Required]
        public RiskLevel RiskLevel { get; set; }
        public string Description { get; set; }
    }

    public class FundDto
    {
        public int FundId { get; set; }
        public string FundName { get; set; }
        public FundType FundType { get; set; }
        public decimal CurrentNAV { get; set; }
        public decimal ExpenseRatio { get; set; }
        public RiskLevel RiskLevel { get; set; }
        public string Description { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
