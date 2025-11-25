using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using ULIP_proj.Interfaces;
using ULIP_proj.Models;

namespace ULIP_proj.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendPolicyProposalEmailAsync(string customerEmail, string customerName, string policyNumber)
        {
            var subject = "New Policy Proposal - Action Required";
            var body = $@"
                <h2>Dear {customerName},</h2>
                <p>A new policy proposal has been created for you.</p>
                <p><strong>Policy Number:</strong> {policyNumber}</p>
                <p>Please log in to your account to review and accept/reject this proposal.</p>
                <p>Best regards,<br/>ULIP Insurance Team</p>";

            await SendEmailAsync(customerEmail, subject, body);
        }

        public async Task SendPolicyAcceptanceEmailAsync(string customerEmail, string customerName, string policyNumber)
        {
            var subject = "Policy Activated Successfully";
            var body = $@"
                <h2>Dear {customerName},</h2>
                <p>Congratulations! Your policy has been activated successfully.</p>
                <p><strong>Policy Number:</strong> {policyNumber}</p>
                <p>You can now start making premium payments and manage your policy online.</p>
                <p>Best regards,<br/>ULIP Insurance Team</p>";

            await SendEmailAsync(customerEmail, subject, body);
        }

        public async Task SendPremiumReminderEmailAsync(string customerEmail, string customerName, string policyNumber, decimal amount, DateTime dueDate)
        {
            var subject = "Premium Payment Reminder";
            var body = $@"
                <h2>Dear {customerName},</h2>
                <p>This is a reminder that your premium payment is due.</p>
                <p><strong>Policy Number:</strong> {policyNumber}</p>
                <p><strong>Amount Due:</strong> ₹{amount:N2}</p>
                <p><strong>Due Date:</strong> {dueDate:dd/MM/yyyy}</p>
                <p>Please make your payment to keep your policy active.</p>
                <p>Best regards,<br/>ULIP Insurance Team</p>";

            await SendEmailAsync(customerEmail, subject, body);
        }

        public async Task SendSurrenderRequestEmailAsync(string managerEmail, string customerName, string policyNumber, decimal amount)
        {
            var subject = "Policy Surrender Request - Approval Required";
            var body = $@"
                <h2>Policy Surrender Request</h2>
                <p>A policy surrender request requires your approval.</p>
                <p><strong>Customer:</strong> {customerName}</p>
                <p><strong>Policy Number:</strong> {policyNumber}</p>
                <p><strong>Surrender Amount:</strong> ₹{amount:N2}</p>
                <p>Please log in to the system to review and approve/reject this request.</p>
                <p>Best regards,<br/>ULIP Insurance System</p>";

            await SendEmailAsync(managerEmail, subject, body);
        }

        public async Task SendApprovalNotificationEmailAsync(string customerEmail, string customerName, string requestType, bool approved, string comments)
        {
            var status = approved ? "Approved" : "Rejected";
            var subject = $"Request {status} - {requestType}";
            var body = $@"
                <h2>Dear {customerName},</h2>
                <p>Your {requestType} request has been {status.ToLower()}.</p>
                {(approved ? "<p>Your request has been processed successfully.</p>" : "")}
                {(!string.IsNullOrEmpty(comments) ? $"<p><strong>Comments:</strong> {comments}</p>" : "")}
                <p>Best regards,<br/>ULIP Insurance Team</p>";

            await SendEmailAsync(customerEmail, subject, body);
        }

        public async Task SendWelcomeEmailAsync(string userEmail, string userName, string role, string tempPassword)
        {
            var subject = "Welcome to ULIP Insurance Portal";
            var body = $"<h2>Welcome {userName}!</h2>" +
                      $"<p>Your account has been created successfully in the ULIP Insurance Portal.</p>" +
                      $"<p><strong>Role:</strong> {role}</p>" +
                      $"<p><strong>Login Email:</strong> {userEmail}</p>" +
                      $"<p><strong>Temporary Password:</strong> {tempPassword}</p>" +
                      $"<p><strong>Login URL:</strong> <a href=\"http://localhost:5173/login\">Click here to login</a></p>" +
                      $"<p><em>Please change your password after first login for security.</em></p>" +
                      $"<p>Best regards,<br/>ULIP Insurance Team</p>";

            await SendEmailAsync(userEmail, subject, body);
        }

        public async Task SendCommissionEarnedEmailAsync(string agentEmail, string agentName, decimal commissionAmount)
        {
            var subject = "Commission Earned - Policy Accepted";
            var body = $@"
                <h2>Dear {agentName},</h2>
                <p>Congratulations! You have earned a commission from a policy acceptance.</p>
                <p><strong>Commission Amount:</strong> ₹{commissionAmount:N2}</p>
                <p>This commission has been added to your account.</p>
                <p>Best regards,<br/>ULIP Insurance Team</p>";

            await SendEmailAsync(agentEmail, subject, body);
        }

        public async Task SendFundSwitchConfirmationEmailAsync(string customerEmail, string customerName, string fromFund, string toFund, decimal amount)
        {
            var subject = "Fund Switch Confirmation";
            var body = $@"
                <h2>Dear {customerName},</h2>
                <p>Your fund switch has been processed successfully.</p>
                <p><strong>From Fund:</strong> {fromFund}</p>
                <p><strong>To Fund:</strong> {toFund}</p>
                <p><strong>Amount:</strong> ₹{amount:N2}</p>
                <p>You can view the updated portfolio in your account.</p>
                <p>Best regards,<br/>ULIP Insurance Team</p>";

            await SendEmailAsync(customerEmail, subject, body);
        }

        public async Task SendSurrenderApprovalRequestEmailAsync(string managerEmail, string customerName, string policyNumber, decimal surrenderValue, string reason)
        {
            var subject = "Policy Surrender Request - Manager Approval Required";
            var body = $@"
                <h2>Policy Surrender Approval Request</h2>
                <p>A policy surrender request requires your approval.</p>
                <p><strong>Customer:</strong> {customerName}</p>
                <p><strong>Policy Number:</strong> {policyNumber}</p>
                <p><strong>Surrender Value:</strong> ₹{surrenderValue:N2}</p>
                <p><strong>Reason:</strong> {reason}</p>
                <p>Please log in to the manager portal to review and approve/reject this request.</p>
                <p>Best regards,<br/>ULIP Insurance System</p>";

            await SendEmailAsync(managerEmail, subject, body);
        }

        public async Task SendSurrenderCompletedEmailAsync(string customerEmail, string customerName, string policyNumber, decimal payoutAmount, bool approved)
        {
            var status = approved ? "Approved" : "Rejected";
            var subject = $"Policy Surrender {status}";
            var body = approved ? $@"
                <h2>Dear {customerName},</h2>
                <p>Your policy surrender request has been approved and processed.</p>
                <p><strong>Policy Number:</strong> {policyNumber}</p>
                <p><strong>Payout Amount:</strong> ₹{payoutAmount:N2}</p>
                <p>The amount will be credited to your registered bank account within 3-5 business days.</p>
                <p>Thank you for choosing ULIP Insurance.</p>
                <p>Best regards,<br/>ULIP Insurance Team</p>" : $@"
                <h2>Dear {customerName},</h2>
                <p>Your policy surrender request has been rejected.</p>
                <p><strong>Policy Number:</strong> {policyNumber}</p>
                <p>Your policy remains active. Please contact customer service for more details.</p>
                <p>Best regards,<br/>ULIP Insurance Team</p>";

            await SendEmailAsync(customerEmail, subject, body);
        }

        public async Task SendMaturityCompletedEmailAsync(string customerEmail, string customerName, string policyNumber, decimal maturityAmount)
        {
            var subject = "Policy Maturity - Congratulations!";
            var body = $@"
                <h2>Dear {customerName},</h2>
                <p>Congratulations! Your ULIP policy has reached maturity.</p>
                <p><strong>Policy Number:</strong> {policyNumber}</p>
                <p><strong>Maturity Amount:</strong> ₹{maturityAmount:N2}</p>
                <p>The maturity amount will be credited to your registered bank account within 3-5 business days.</p>
                <p>Thank you for your trust in ULIP Insurance over the years.</p>
                <p>Best regards,<br/>ULIP Insurance Team</p>";

            await SendEmailAsync(customerEmail, subject, body);
        }

        public async Task SendPolicyLapseNotificationAsync(string customerEmail, string customerName, string policyNumber, decimal premiumAmount, DateTime dueDate)
        {
            var subject = "Policy Lapse Notification - Immediate Action Required";
            var body = $@"
                <h2>Dear {customerName},</h2>
                <p><strong>IMPORTANT:</strong> Your policy has lapsed due to non-payment of premium.</p>
                <p><strong>Policy Number:</strong> {policyNumber}</p>
                <p><strong>Overdue Premium:</strong> ₹{premiumAmount:N2}</p>
                <p><strong>Original Due Date:</strong> {dueDate:dd/MM/yyyy}</p>
                <p>Your policy benefits are now suspended. Please contact us immediately to discuss revival options.</p>
                <p><strong>Revival Period:</strong> You have up to 2 years to revive your policy.</p>
                <p>Best regards,<br/>ULIP Insurance Team</p>";

            await SendEmailAsync(customerEmail, subject, body);
        }

        public async Task SendDocumentVerificationEmailAsync(string customerEmail, string customerName, string policyNumber, string documentType, bool approved, string? comments)
        {
            var status = approved ? "Approved" : "Rejected";
            var subject = $"Document Verification {status} - {documentType}";
            var body = approved ? $@"
                <h2>Dear {customerName},</h2>
                <p>Your document has been verified and approved.</p>
                <p><strong>Policy Number:</strong> {policyNumber}</p>
                <p><strong>Document Type:</strong> {documentType}</p>
                <p><strong>Status:</strong> {status}</p>
                {(!string.IsNullOrEmpty(comments) ? $"<p><strong>Comments:</strong> {comments}</p>" : "")}
                <p>Your policy processing will continue as planned.</p>
                <p>Best regards,<br/>ULIP Insurance Team</p>" : $@"
                <h2>Dear {customerName},</h2>
                <p>Your document verification has been rejected.</p>
                <p><strong>Policy Number:</strong> {policyNumber}</p>
                <p><strong>Document Type:</strong> {documentType}</p>
                <p><strong>Status:</strong> {status}</p>
                {(!string.IsNullOrEmpty(comments) ? $"<p><strong>Reason:</strong> {comments}</p>" : "")}
                <p>Please upload a corrected document or contact customer service for assistance.</p>
                <p>Best regards,<br/>ULIP Insurance Team</p>";

            await SendEmailAsync(customerEmail, subject, body);
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                _logger.LogInformation("Attempting to send email to {Email} with subject: {Subject}", toEmail, subject);
                _logger.LogInformation("SMTP Settings - Host: {Host}, Port: {Port}, From: {From}", _emailSettings.SmtpHost, _emailSettings.SmtpPort, _emailSettings.FromEmail);
                
                using var client = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort);
                client.EnableSsl = _emailSettings.EnableSsl;
                client.Credentials = new NetworkCredential(_emailSettings.FromEmail, _emailSettings.Password);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);
                await client.SendMailAsync(mailMessage);
                
                _logger.LogInformation("✅ Email sent successfully to {Email}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to send email to {Email}. Error: {Error}", toEmail, ex.Message);

            }
        }
    }
}
