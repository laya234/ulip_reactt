namespace ULIP_proj.Interfaces
{
    public interface IEmailService
    {
        Task SendPolicyProposalEmailAsync(string customerEmail, string customerName, string policyNumber);
        Task SendPolicyAcceptanceEmailAsync(string customerEmail, string customerName, string policyNumber);
        Task SendPremiumReminderEmailAsync(string customerEmail, string customerName, string policyNumber, decimal amount, DateTime dueDate);
        Task SendSurrenderRequestEmailAsync(string managerEmail, string customerName, string policyNumber, decimal amount);
        Task SendApprovalNotificationEmailAsync(string customerEmail, string customerName, string requestType, bool approved, string comments);
        Task SendWelcomeEmailAsync(string userEmail, string userName, string role, string tempPassword);
        Task SendCommissionEarnedEmailAsync(string agentEmail, string agentName, decimal commissionAmount);
        Task SendFundSwitchConfirmationEmailAsync(string customerEmail, string customerName, string fromFund, string toFund, decimal amount);
        Task SendSurrenderApprovalRequestEmailAsync(string managerEmail, string customerName, string policyNumber, decimal surrenderValue, string reason);
        Task SendSurrenderCompletedEmailAsync(string customerEmail, string customerName, string policyNumber, decimal payoutAmount, bool approved);
        Task SendMaturityCompletedEmailAsync(string customerEmail, string customerName, string policyNumber, decimal maturityAmount);
        Task SendPolicyLapseNotificationAsync(string customerEmail, string customerName, string policyNumber, decimal premiumAmount, DateTime dueDate);
        Task SendDocumentVerificationEmailAsync(string customerEmail, string customerName, string policyNumber, string documentType, bool approved, string? comments);
    }
}
