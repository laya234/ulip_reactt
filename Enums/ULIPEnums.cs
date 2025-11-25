namespace ULIP_proj.Enums
{
    public enum PremiumFrequency
    {
        Monthly,
        Quarterly,
        HalfYearly,
        Yearly
    }

    public enum PolicyStatus
    {
        Proposed,
        PendingAcceptance,
        Active,
        Lapsed,
        SurrenderRequested,
        Matured,
        Surrendered,
        Rejected
    }

    public enum FundType
    {
        Equity,
        Debt,
        Hybrid,
        Balanced
    }

    public enum RiskLevel
    {
        Low,
        Medium,
        High
    }

    public enum TransactionType
    {
        Purchase,
        Redemption,
        Switch
    }

    public enum PremiumStatus
    {
        Paid,
        Pending,
        Overdue
    }

    public enum PaymentMethod
    {
        Online,
        Cheque,
        Cash,
        AutoDebit
    }

    public enum UserRole
    {
        Customer,
        Agent,
        Admin,
        Manager
    }


    public enum ApprovalStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public enum SurrenderStatus
    {
        Requested,
        UnderReview,
        Approved,
        Processed,
        Rejected
    }

    public enum SaleStatus
    {
        Lead,
        Quoted,
        Converted,
        Lost
    }

    public enum ApprovalType
    {
        PolicySurrender,
        HighValueTransaction,
        FundSwitch,
        PremiumWaiver
    }

    public enum DocumentType
    {
        PanCard,
        AadhaarCard,
        AddressProof,
        IncomeProof,
        MedicalReport,
        BankDetails,
        SurrenderForm,
        MaturityClaimForm
    }

    public enum DocumentStatus
    {
        Pending,
        Uploaded,
        Verified,
        Rejected
    }

}
