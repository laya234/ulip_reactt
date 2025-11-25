using System.ComponentModel.DataAnnotations;
using ULIP_proj.Enums;

namespace ULIP_proj.DTOs
{
    public class CreatePremiumDto
    {
        [Required]
        public int PolicyId { get; set; }
        [Required]
        public decimal PremiumAmount { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string TransactionReference { get; set; }
    }

    public class PayPremiumDto
    {
        [Required]
        public int PolicyId { get; set; }
        [Required]
        public int FundId { get; set; }
        [Required]
        public decimal PremiumAmount { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
        [Required]
        public PaymentMethod PaymentMethod { get; set; }
    }

    public class PremiumDto
    {
        public int PremiumId { get; set; }
        public int PolicyId { get; set; }
        public int FundId { get; set; }
        public decimal PremiumAmount { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? PaidDate { get; set; }
        public PremiumStatus Status { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public string TransactionReference { get; set; }
        public string PolicyNumber { get; set; }
        public string CustomerName { get; set; }
    }
}
