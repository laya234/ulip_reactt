using System.ComponentModel.DataAnnotations;
using ULIP_proj.Enums;

namespace ULIP_proj.DTOs
{
    public class CreatePolicyDto
    {
        [Required]
        public string PolicyNumber { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
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

    public class PolicyDto
    {
        public int PolicyId { get; set; }
        public string PolicyNumber { get; set; }
        public string PolicyName { get; set; }
        public int UserId { get; set; }
        public decimal SumAssured { get; set; }
        public decimal PremiumAmount { get; set; }
        public PremiumFrequency PremiumFrequency { get; set; }
        public DateTime PolicyStartDate { get; set; }
        public DateTime PolicyMaturityDate { get; set; }
        public PolicyStatus PolicyStatus { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal TotalPremiumPaid { get; set; }
        public string UserName { get; set; }
    }


}