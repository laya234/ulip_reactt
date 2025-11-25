using System.ComponentModel.DataAnnotations;
using ULIP_proj.Enums;

namespace ULIP_proj.DTOs
{
    public class CreateTransactionDto
    {
        [Required]
        public int PolicyId { get; set; }
        [Required]
        public int FundId { get; set; }
        [Required]
        public TransactionType TransactionType { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public decimal NAV { get; set; }
        public string Description { get; set; }
    }

    public class TransactionDto
    {
        public int TransactionId { get; set; }
        public int PolicyId { get; set; }
        public int FundId { get; set; }
        public TransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public decimal Units { get; set; }
        public decimal NAV { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }
        public string PolicyNumber { get; set; }
        public string FundName { get; set; }
    }
}
