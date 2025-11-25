using ULIP_proj.Enums;

namespace ULIP_proj.DTOs
{
    public class ApprovalDto
    {
        public int ApprovalId { get; set; }
        public int RequestId { get; set; }
        public ApprovalType RequestType { get; set; }
        public int RequestedBy { get; set; }
        public string RequestedByName { get; set; }
        public string RequestedByEmail { get; set; }
        public ApprovalStatus Status { get; set; }
        public decimal? Amount { get; set; }
        public string RequestReason { get; set; }
        public string ApprovalComments { get; set; }
        public DateTime RequestedAt { get; set; }
    }

    public class SaleDto
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public int? PolicyId { get; set; }
        public SaleStatus Status { get; set; }
        public decimal QuotedAmount { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class CreateSaleDto
    {
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public decimal QuotedAmount { get; set; }
        public string Notes { get; set; }
    }
}

public class DocumentDto
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public string DocumentType { get; set; }
    public string UploadDate { get; set; }
    public string Size { get; set; }
}

public class PortfolioValueDto
{
    public List<FundHoldingDto> FundHoldings { get; set; }
    public decimal TotalInvested { get; set; }
    public decimal TotalCurrentValue { get; set; }
    public decimal TotalGainLoss { get; set; }
    public decimal ReturnPercentage { get; set; }
}

public class FundHoldingDto
{
    public int FundId { get; set; }
    public string FundName { get; set; }
    public decimal TotalUnits { get; set; }
    public decimal TotalInvested { get; set; }
    public decimal CurrentNAV { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal GainLoss { get; set; }
    public decimal ReturnPercentage { get; set; }
}

public class FundDetailsDto
{
    public int FundId { get; set; }
    public string FundName { get; set; }
    public string FundType { get; set; }
    public decimal CurrentNAV { get; set; }
    public decimal ExpenseRatio { get; set; }
    public string RiskLevel { get; set; }
    public string Description { get; set; }
    public decimal OneYearReturn { get; set; }
    public decimal ThreeYearReturn { get; set; }
    public decimal FiveYearReturn { get; set; }
    public string Benefits { get; set; }
    public string SuitableFor { get; set; }
}
