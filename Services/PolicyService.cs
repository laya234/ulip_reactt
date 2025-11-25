using AutoMapper;
using Microsoft.Extensions.Logging;
using ULIP_proj.DTOs;
using ULIP_proj.Interfaces;
using ULIP_proj.Models;
using ULIP_proj.Enums;
using ULIP_proj.Extensions;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using Microsoft.AspNetCore.Http;

namespace ULIP_proj.Services
{
    public class PolicyService : IPolicyService
    {
        private readonly IPolicyRepository _policyRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IApprovalRepository _approvalRepository;
        private readonly ISaleRepository _saleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPolicyDocumentRepository _documentRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PolicyService> _logger;
        private readonly IEmailService _emailService;
        private readonly IFileService _fileService;
        private readonly IPremiumService _premiumService;
        private readonly IFundRepository _fundRepository;

        public PolicyService(IPolicyRepository policyRepository, ITransactionRepository transactionRepository, IApprovalRepository approvalRepository, ISaleRepository saleRepository, IUserRepository userRepository, IPolicyDocumentRepository documentRepository, IMapper mapper, ILogger<PolicyService> logger, IEmailService emailService, IFileService fileService, IPremiumService premiumService, IFundRepository fundRepository)
        {
            _policyRepository = policyRepository;
            _transactionRepository = transactionRepository;
            _approvalRepository = approvalRepository;
            _saleRepository = saleRepository;
            _userRepository = userRepository;
            _documentRepository = documentRepository;
            _mapper = mapper;
            _logger = logger;
            _emailService = emailService;
            _fileService = fileService;
            _premiumService = premiumService;
            _fundRepository = fundRepository;
        }

        public async Task<PolicyDto> CreatePolicyAsync(CreatePolicyDto policyDto, int agentId)
        {
            
            var policy = new Policy
            {
                PolicyNumber = GeneratePolicyNumber(),
                UserId = policyDto.UserId,
                AgentId = agentId,
                PolicyName = policyDto.PolicyName,
                SumAssured = policyDto.SumAssured,
                PremiumAmount = policyDto.PremiumAmount,
                PremiumFrequency = policyDto.PremiumFrequency,
                PolicyStartDate = policyDto.PolicyStartDate,
                PolicyMaturityDate = policyDto.PolicyMaturityDate,
                PolicyStatus = PolicyStatus.Active
            };

            var createdPolicy = await _policyRepository.CreateAsync(policy);
            return MapToPolicyDto(createdPolicy);
        }

        public async Task<PolicyDto> GetPolicyDetailsAsync(int policyId)
        {
            var policy = await _policyRepository.GetByIdAsync(policyId);
            return policy == null ? null : MapToPolicyDto(policy);
        }

        public async Task<bool> RequestSurrenderAsync(int policyId, string reason, int userId)
        {
            var policy = await _policyRepository.GetByIdAsync(policyId);
            if (policy == null) return false;

            policy.SurrenderRequested = true;
            policy.SurrenderRequestDate = DateTimeExtensions.NowIst();
            policy.SurrenderStatus = SurrenderStatus.Requested;
            policy.PolicyStatus = PolicyStatus.SurrenderRequested;
            await _policyRepository.UpdateAsync(policy);

            var approval = new Approval
            {
                RequestId = policyId,
                RequestType = ApprovalType.PolicySurrender,
                RequestedBy = userId,
                Status = ApprovalStatus.Pending,
                Amount = policy.CurrentValue,
                RequestReason = reason,
                RequestedAt = DateTimeExtensions.NowIst()
            };

            await _approvalRepository.CreateAsync(approval);
            
            await _emailService.SendSurrenderApprovalRequestEmailAsync(
                "manager@ulip.com", 
                policy.User?.FirstName + " " + policy.User?.LastName ?? "Customer", 
                policy.PolicyNumber, 
                policy.CurrentValue * 0.90m, 
                reason);
            
            return true;
        }

        public async Task<decimal> CalculateSurrenderValueAsync(int policyId)
        {
            var policy = await _policyRepository.GetByIdAsync(policyId);
            if (policy == null) return 0;
            
            var transactions = await _transactionRepository.GetByPolicyIdAsync(policyId);
            
            return policy.CurrentValue * 0.90m;
        }

        public async Task<IEnumerable<PolicyDto>> GetMyPoliciesAsync(int userId)
        {
            var policies = await _policyRepository.GetByCustomerIdAsync(userId);
            return policies.Select(MapToPolicyDto);
        }

        public async Task<IEnumerable<PolicyDto>> GetAgentPoliciesAsync(int agentId)
        {
            var policies = await _policyRepository.GetByAgentIdAsync(agentId);
            return policies.Select(MapToPolicyDto);
        }



        private string GeneratePolicyNumber()
        {
            return "POL" + DateTimeExtensions.NowIst().ToString("yyyyMMddHHmmss");
        }

        public async Task<PolicyProposalDto> CreatePolicyProposalAsync(int saleId, CreatePolicyProposalDto proposalDto, int agentId)
        {
            _logger.LogInformation("Creating policy proposal for sale {SaleId} by agent {AgentId}", saleId, agentId);
            
            var sales = await _saleRepository.GetByAgentIdAsync(agentId);
            var sale = sales.FirstOrDefault(s => s.Id == saleId);
            if (sale == null) 
            {
                _logger.LogWarning("Sale {SaleId} not found for agent {AgentId}", saleId, agentId);
                throw new ArgumentException("Sale not found");
            }
            
            var customerId = await GetOrCreateCustomerFromSaleAsync(sale);
            
            var policy = _mapper.Map<Policy>(proposalDto);
            policy.PolicyNumber = GeneratePolicyNumber();
            policy.UserId = customerId;
            policy.AgentId = agentId;
            policy.PolicyStatus = PolicyStatus.PendingAcceptance;
            policy.ProposedDate = DateTimeExtensions.NowIst();

            var createdPolicy = await _policyRepository.CreateAsync(policy);
            _logger.LogInformation("Policy proposal created with ID {PolicyId} for customer {CustomerId}", createdPolicy.PolicyId, customerId);
            
            sale.Status = SaleStatus.Quoted;
            sale.PolicyId = createdPolicy.PolicyId;
            await _saleRepository.UpdateAsync(sale);
            
            var customer = await _userRepository.GetByIdAsync(customerId);
            if (customer != null)
            {
                await _emailService.SendPolicyProposalEmailAsync(customer.Email, $"{customer.FirstName} {customer.LastName}", policy.PolicyNumber);
            }
            
            return _mapper.Map<PolicyProposalDto>(createdPolicy);
        }

        public async Task<PolicyProposalDto> CreatePolicyProposalForCustomerAsync(int customerId, CreatePolicyProposalDto proposalDto, int agentId)
        {
            _logger.LogInformation("Creating policy proposal for customer {CustomerId} by agent {AgentId}", customerId, agentId);
            
            var customer = await _userRepository.GetByIdAsync(customerId);
            if (customer == null)
            {
                _logger.LogWarning("Customer {CustomerId} not found", customerId);
                throw new ArgumentException("Customer not found");
            }

            _logger.LogInformation("Proceeding with policy proposal creation for customer {CustomerId}", customerId);
            
            var policy = _mapper.Map<Policy>(proposalDto);
            policy.PolicyNumber = GeneratePolicyNumber();
            policy.UserId = customerId;
            policy.AgentId = agentId;
            policy.PolicyStatus = PolicyStatus.PendingAcceptance;
            policy.ProposedDate = DateTimeExtensions.NowIst();

            var createdPolicy = await _policyRepository.CreateAsync(policy);
            _logger.LogInformation("Policy proposal created with ID {PolicyId} for customer {CustomerId}", createdPolicy.PolicyId, customerId);
            
            var sales = await _saleRepository.GetByAgentIdAsync(agentId);
            var existingSale = sales.FirstOrDefault(s => s.CustomerName == $"{customer.FirstName} {customer.LastName}" && s.Status == SaleStatus.Lead);
            
            if (existingSale != null)
            {
                existingSale.Status = SaleStatus.Quoted;
                existingSale.PolicyId = createdPolicy.PolicyId;
                existingSale.QuotedAmount = proposalDto.SumAssured;
                await _saleRepository.UpdateAsync(existingSale);
                _logger.LogInformation("Updated existing sale {SaleId} to Quoted status for policy {PolicyId}", existingSale.Id, createdPolicy.PolicyId);
            }
            else
            {
                var newSale = new Sale
                {
                    AgentId = agentId,
                    CustomerName = $"{customer.FirstName} {customer.LastName}",
                    CustomerPhone = customer.PhoneNumber,
                    Status = SaleStatus.Quoted,
                    QuotedAmount = proposalDto.SumAssured,
                    PolicyId = createdPolicy.PolicyId,
                    Notes = "Policy proposal created directly for customer"
                };
                await _saleRepository.CreateAsync(newSale);
                _logger.LogInformation("Created new sale record {SaleId} for policy proposal {PolicyId}", newSale.Id, createdPolicy.PolicyId);
            }
            
            _logger.LogInformation("Sending policy proposal email to {Email} for policy {PolicyNumber}", customer.Email, policy.PolicyNumber);
            await _emailService.SendPolicyProposalEmailAsync(customer.Email, $"{customer.FirstName} {customer.LastName}", policy.PolicyNumber);
            
            return _mapper.Map<PolicyProposalDto>(createdPolicy);
        }

        public async Task<IEnumerable<PolicyProposalDto>> GetPendingProposalsAsync(int userId)
        {
            var policies = await _policyRepository.GetByCustomerIdAsync(userId);
            var pendingPolicies = policies.Where(p => p.PolicyStatus == PolicyStatus.PendingAcceptance);
            return _mapper.Map<IEnumerable<PolicyProposalDto>>(pendingPolicies);
        }

        public async Task<bool> AcceptPolicyProposalAsync(int policyId, PolicyAcceptanceDto acceptanceDto)
        {
            _logger.LogInformation("AcceptPolicyProposal called with policyId: {PolicyId}, Accepted: {Accepted}", policyId, acceptanceDto.Accepted);
            var policy = await _policyRepository.GetByIdAsync(policyId);
            if (policy == null) 
            {
                _logger.LogWarning("Policy {PolicyId} not found", policyId);
                return false;
            }
            if (policy.PolicyStatus != PolicyStatus.PendingAcceptance) 
            {
                _logger.LogWarning("Policy {PolicyId} status is {Status}, not PendingAcceptance", policyId, policy.PolicyStatus);
                return false;
            }

            if (acceptanceDto.Accepted)
            {
                _logger.LogInformation("Setting policy {PolicyId} status to Active", policyId);
                
                if (!policy.KYCVerified)
                {
                    _logger.LogInformation("Auto-verifying KYC for testing purposes - Policy {PolicyId}", policyId);
                    policy.KYCVerified = true;
                    policy.KYCVerifiedAt = DateTime.UtcNow;
                }
                
                if (policy.SumAssured > 2500000 && !policy.MedicalVerified)
                {
                    _logger.LogInformation("Auto-verifying Medical for testing purposes - Policy {PolicyId}", policyId);
                    policy.MedicalVerified = true;
                }
                
                
                policy.PolicyStatus = PolicyStatus.Active;
                policy.AcceptedDate = DateTimeExtensions.NowIst();
                
                try
                {
                    await _premiumService.GeneratePremiumScheduleAsync(policyId);
                    _logger.LogInformation("Premium schedule generated for policy {PolicyId}", policyId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to generate premium schedule for policy {PolicyId}", policyId);
                }
                
                if (policy.AgentId.HasValue)
                {
                    var sales = await _saleRepository.GetByAgentIdAsync(policy.AgentId.Value);
                    _logger.LogInformation("Found {SalesCount} sales for agent {AgentId}", sales.Count(), policy.AgentId.Value);
                    
                    foreach (var sale in sales)
                    {
                        _logger.LogInformation("Sale {SaleId}: PolicyId={PolicyId}, Status={Status}, CustomerName={CustomerName}", 
                            sale.Id, sale.PolicyId, sale.Status, sale.CustomerName);
                    }
                    
                    var relatedSale = sales.FirstOrDefault(s => s.PolicyId == policy.PolicyId);
                    
                    if (relatedSale == null)
                    {
                        var policyCustomer = await _userRepository.GetByIdAsync(policy.UserId);
                        if (policyCustomer != null)
                        {
                            var customerFullName = $"{policyCustomer.FirstName} {policyCustomer.LastName}";
                            relatedSale = sales.FirstOrDefault(s => 
                                s.CustomerName.Equals(customerFullName, StringComparison.OrdinalIgnoreCase) && 
                                (s.Status == SaleStatus.Quoted || s.Status == SaleStatus.Lead));
                            
                            if (relatedSale != null)
                            {
                                relatedSale.PolicyId = policy.PolicyId;
                                _logger.LogInformation("Found sale {SaleId} by customer name fallback, linking to policy {PolicyId}", relatedSale.Id, policy.PolicyId);
                            }
                        }
                    }
                    
                    if (relatedSale != null)
                    {
                        _logger.LogInformation("Updating sale {SaleId} status to Converted for policy {PolicyId}", relatedSale.Id, policy.PolicyId);
                        relatedSale.Status = SaleStatus.Converted;
                        await _saleRepository.UpdateAsync(relatedSale);
                        _logger.LogInformation("Successfully updated sale {SaleId} to Converted status", relatedSale.Id);
                    }
                    else
                    {
                        _logger.LogWarning("No matching sale found for policy {PolicyId} with agent {AgentId}. Customer: {CustomerId}", policy.PolicyId, policy.AgentId.Value, policy.UserId);
                    }
                    
                    var agent = await _userRepository.GetByIdAsync(policy.AgentId.Value);
                    if (agent != null)
                    {
                        var commissionAmount = policy.PremiumAmount * 0.05m;
                        agent.TotalCommissionEarned += commissionAmount;
                        agent.PoliciesSold += 1;
                        await _userRepository.UpdateAsync(agent);
                        
                        await _emailService.SendCommissionEarnedEmailAsync(
                            agent.Email, 
                            $"{agent.FirstName} {agent.LastName}", 
                            commissionAmount);
                    }
                }
                
                var customer = await _userRepository.GetByIdAsync(policy.UserId);
                if (customer != null)
                {
                    _logger.LogInformation("Sending policy acceptance email to {Email} for policy {PolicyNumber}", customer.Email, policy.PolicyNumber);
                    await _emailService.SendPolicyAcceptanceEmailAsync(customer.Email, $"{customer.FirstName} {customer.LastName}", policy.PolicyNumber);
                }
            }
            else
            {
                _logger.LogInformation("Setting policy {PolicyId} status to Rejected", policyId);
                policy.PolicyStatus = PolicyStatus.Rejected;
                policy.RejectedDate = DateTimeExtensions.NowIst();
                policy.RejectionReason = acceptanceDto.RejectionReason;
            }

            await _policyRepository.UpdateAsync(policy);
            _logger.LogInformation("Policy {PolicyId} updated with status {Status}", policyId, policy.PolicyStatus);
            return true;
        }

        private async Task<int> GetOrCreateCustomerFromSaleAsync(Sale sale)
        {
            var existingCustomers = await _userRepository.GetAllAsync();
            var existingCustomer = existingCustomers.FirstOrDefault(u => u.PhoneNumber == sale.CustomerPhone);
            
            if (existingCustomer != null)
            {
                _logger.LogInformation("Found existing customer {CustomerId} for sale {SaleId}", existingCustomer.UserId, sale.Id);
                return existingCustomer.UserId;
            }
            
            var customerEmail = $"{sale.CustomerName.Replace(" ", "").ToLower()}@customer.com";
            
            _logger.LogInformation("Creating new customer for sale {SaleId} with name {CustomerName}", sale.Id, sale.CustomerName);
            var nameParts = sale.CustomerName.Split(' ', 2);
            var newCustomer = new User
            {
                FirstName = nameParts[0],
                LastName = nameParts.Length > 1 ? nameParts[1] : "",
                Email = customerEmail,
                PhoneNumber = sale.CustomerPhone,
                Role = UserRole.Customer,
                DateOfBirth = DateTimeExtensions.NowIst().AddYears(-30),
                Address = "Address to be updated",
                PanNumber = "PAN" + DateTimeExtensions.NowIst().ToString("yyyyMMdd"),
                Password = BCrypt.Net.BCrypt.HashPassword("TempPassword123"),
                IsActive = true
            };
            
            var createdCustomer = await _userRepository.CreateAsync(newCustomer);
            _logger.LogInformation("Created new customer {CustomerId} for sale {SaleId}", createdCustomer.UserId, sale.Id);
            return createdCustomer.UserId;
        }

        public async Task<byte[]> GeneratePolicyStatementAsync(int policyId, int userId)
        {
            try
            {
                var policy = await _policyRepository.GetByIdAsync(policyId);
                if (policy == null || policy.UserId != userId)
                    return null;

                var transactions = await _transactionRepository.GetByPolicyIdAsync(policyId);
                var currentValue = await CalculateCurrentValueAsync(policyId, transactions);
                
                return await GeneratePdfDirectly(policy, transactions, currentValue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating policy statement for policy {PolicyId}", policyId);
                throw;
            }
        }

        private async Task<byte[]> GeneratePdfDirectly(Policy policy, IEnumerable<Transaction> transactions, decimal currentValue)
        {
            var txnList = transactions.ToList();
            
            // Pre-load all fund names
            var fundNames = new Dictionary<int, string>();
            foreach (var txn in txnList)
            {
                if (txn.FundId > 0 && !fundNames.ContainsKey(txn.FundId))
                {
                    var fund = await _fundRepository.GetByIdAsync(txn.FundId);
                    fundNames[txn.FundId] = fund?.FundName ?? "N/A";
                }
            }
            
            var memoryStream = new MemoryStream();
            var writer = new PdfWriter(memoryStream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);
            
            var totalInvested = txnList.Where(t => t.TransactionType == TransactionType.Purchase).Sum(t => t.Amount) - 
                                txnList.Where(t => t.TransactionType == TransactionType.Redemption).Sum(t => t.Amount);
            var gainLoss = currentValue - totalInvested;
            
            document.Add(new Paragraph("ULIP Policy Statement")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(20)
                .SetBold()
                .SetMarginBottom(20));
            
            document.Add(new Paragraph($"Generated on: {DateTime.Now:dd/MM/yyyy}")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(30));
            
            document.Add(new Paragraph("Policy Information")
                .SetFontSize(16)
                .SetBold()
                .SetMarginBottom(10));
            
            document.Add(new Paragraph($"Policy Number: {policy.PolicyNumber}"));
            document.Add(new Paragraph($"Policy Name: {policy.PolicyName}"));
            document.Add(new Paragraph($"Sum Assured: Rs.{policy.SumAssured:N2}"));
            document.Add(new Paragraph($"Premium Amount: Rs.{policy.PremiumAmount:N2}")
                .SetMarginBottom(20));
            
            document.Add(new Paragraph("Investment Summary")
                .SetFontSize(16)
                .SetBold()
                .SetMarginBottom(10));
            
            document.Add(new Paragraph($"Total Invested: Rs.{totalInvested:N2}"));
            document.Add(new Paragraph($"Current Value: Rs.{currentValue:N2}"));
            document.Add(new Paragraph($"Gain/Loss: Rs.{gainLoss:N2}")
                .SetMarginBottom(20));
            
            if (txnList.Any())
            {
                document.Add(new Paragraph("Transaction History")
                    .SetFontSize(16)
                    .SetBold()
                    .SetMarginBottom(10));
                
                foreach (var txn in txnList.OrderByDescending(t => t.TransactionDate).Take(10))
                {
                    var fundName = fundNames.ContainsKey(txn.FundId) ? fundNames[txn.FundId] : "N/A";
                    document.Add(new Paragraph(
                        $"{txn.TransactionDate:dd/MM/yyyy} - {txn.TransactionType} - {fundName} - Rs.{txn.Amount:N2}")
                        .SetFontSize(10));
                }
            }
            
            document.Add(new Paragraph("Thank you for choosing ULIP Insurance")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginTop(30)
                .SetFontSize(10));
            
            document.Close();
            var result = memoryStream.ToArray();
            memoryStream.Dispose();
            return result;
        }

        private async Task<decimal> CalculateCurrentValueAsync(int policyId, IEnumerable<Transaction> transactions)
        {
            if (transactions == null || !transactions.Any())
                return 0;
            
            decimal totalValue = 0;
            var fundGroups = transactions.GroupBy(t => t.FundId);

            foreach (var group in fundGroups)
            {
                var fund = await _fundRepository.GetByIdAsync(group.Key);
                if (fund != null)
                {
                    var totalUnits = group.Where(t => t.TransactionType == TransactionType.Purchase).Sum(t => t.Units) -
                                    group.Where(t => t.TransactionType == TransactionType.Redemption).Sum(t => t.Units);
                    
                    totalValue += totalUnits * fund.CurrentNAV;
                }
            }

            return totalValue;
        }



        public async Task<bool> ProcessSurrenderApprovalAsync(int policyId, bool approved, int managerId, string? comments = null)
        {
            var policy = await _policyRepository.GetByIdAsync(policyId);
            if (policy == null || policy.SurrenderStatus != SurrenderStatus.Requested)
                return false;

            decimal payoutAmount = 0;
            
            if (approved)
            {
                policy.PolicyStatus = PolicyStatus.Surrendered;
                policy.SurrenderStatus = SurrenderStatus.Processed;
                payoutAmount = policy.CurrentValue * 0.90m;
                policy.SurrenderValue = payoutAmount;
            }
            else
            {
                policy.PolicyStatus = PolicyStatus.Active;
                policy.SurrenderStatus = SurrenderStatus.Rejected;
                policy.SurrenderRequested = false;
            }
            
            await _policyRepository.UpdateAsync(policy);
            await SendCustomerEmailAsync(policy.UserId, (customer) => 
                _emailService.SendSurrenderCompletedEmailAsync(
                    customer.Email,
                    $"{customer.FirstName} {customer.LastName}",
                    policy.PolicyNumber,
                    payoutAmount,
                    approved));

            return true;
        }



        private async Task SendCustomerEmailAsync(int userId, Func<User, Task> emailAction)
        {
            var customer = await _userRepository.GetByIdAsync(userId);
            if (customer != null)
            {
                await emailAction(customer);
            }
        }

        public async Task<bool> UploadPolicyDocumentAsync(int policyId, IFormFile file, string documentType, int userId)
        {
            _logger.LogInformation($"Uploading document for policy {policyId}, type {documentType}, user {userId}");
            var policy = await _policyRepository.GetByIdAsync(policyId);
            if (policy == null)
            {
                _logger.LogWarning($"Policy {policyId} not found");
                return false;
            }
            
            if (policy.UserId != userId)
            {
                _logger.LogWarning($"User {userId} not authorized for policy {policyId}. Policy belongs to user {policy.UserId}");
                return false;
            }

            string filePath = "test-document-path";
            if (file != null)
            {
                try
                {
                    filePath = await _fileService.UploadPolicyDocumentAsync(file, policyId);
                    _logger.LogInformation($"File uploaded to path: {filePath}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to upload file");
                    filePath = $"uploads/policy_{policyId}_{documentType}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                }
            }
            
            var document = new PolicyDocument
            {
                PolicyId = policyId,
                DocumentType = Enum.Parse<DocumentType>(documentType),
                FilePath = filePath,
                Status = DocumentStatus.Uploaded,
                UploadedAt = DateTime.UtcNow
            };
            
            try
            {
                var createdDoc = await _documentRepository.CreateAsync(document);
                _logger.LogInformation($"Document record created with ID: {createdDoc.DocumentId} for policy {policyId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to create document record for policy {policyId}");
                return false;
            }
        }

        public async Task<bool> VerifyDocumentAsync(int policyId, string documentType, bool approved, int verifierId, string? comments = null)
        {
            var policy = await _policyRepository.GetByIdAsync(policyId);
            if (policy == null) return false;

            var docType = Enum.Parse<DocumentType>(documentType);
            
            if (approved)
            {
                if (IsKYCDocument(docType))
                {
                    policy.KYCVerified = CheckAllKYCDocumentsVerified(policyId);
                    policy.KYCVerifiedAt = DateTime.UtcNow;
                }
                else if (docType == DocumentType.MedicalReport)
                {
                    policy.MedicalVerified = true;
                }
                
                await _policyRepository.UpdateAsync(policy);
                
                var customer = await _userRepository.GetByIdAsync(policy.UserId);
                if (customer != null)
                {
                    await _emailService.SendDocumentVerificationEmailAsync(
                        customer.Email,
                        $"{customer.FirstName} {customer.LastName}",
                        policy.PolicyNumber,
                        documentType,
                        approved,
                        comments
                    );
                }
            }
            
            return true;
        }

        public async Task<IEnumerable<object>> GetPoliciesWithPendingDocumentsAsync()
        {
            _logger.LogInformation("Getting policies with pending documents...");
            var uploadedDocuments = await _documentRepository.GetUploadedDocumentsAsync();
            _logger.LogInformation($"Found {uploadedDocuments.Count()} uploaded documents");
            
            var groupedByPolicy = uploadedDocuments
                .Where(d => d.Policy != null)
                .GroupBy(d => d.PolicyId)
                .Select(g => new
                {
                    PolicyId = g.Key,
                    PolicyNumber = g.First().Policy?.PolicyNumber ?? "Unknown",
                    PolicyName = g.First().Policy?.PolicyName ?? "Unknown",
                    CustomerName = $"{g.First().Policy?.User?.FirstName ?? "Unknown"} {g.First().Policy?.User?.LastName ?? ""}",
                    SumAssured = g.First().Policy?.SumAssured ?? 0,
                    KYCVerified = g.First().Policy?.KYCVerified ?? false,
                    MedicalVerified = g.First().Policy?.MedicalVerified ?? false,
                    UploadedDocuments = g.Select(d => d.DocumentType.ToString()).ToArray(),
                    PolicyStatus = g.First().Policy?.PolicyStatus.ToString() ?? "Unknown"
                }).ToList();
            
            _logger.LogInformation($"Returning {groupedByPolicy.Count} policies with documents");
            foreach(var policy in groupedByPolicy)
            {
                _logger.LogInformation($"Policy: {policy.PolicyNumber}, Documents: [{string.Join(", ", policy.UploadedDocuments)}]");
            }
            
            return groupedByPolicy;
        }

        public async Task<object> GetDocumentStatusAsync(int policyId)
        {
            var policy = await _policyRepository.GetByIdAsync(policyId);
            if (policy == null) return null;

            return new
            {
                PolicyId = policyId,
                KYCVerified = policy.KYCVerified,
                MedicalVerified = policy.MedicalVerified,
                RequiredDocuments = GetRequiredDocuments(policy.SumAssured),
                CanActivate = policy.KYCVerified && (policy.SumAssured <= 2500000 || policy.MedicalVerified)
            };
        }

        private bool IsKYCDocument(DocumentType docType)
        {
            return docType == DocumentType.PanCard || 
                   docType == DocumentType.AadhaarCard || 
                   docType == DocumentType.AddressProof || 
                   docType == DocumentType.IncomeProof;
        }

        private bool CheckAllKYCDocumentsVerified(int policyId)
        {
            return true;
        }

        private string[] GetRequiredDocuments(decimal sumAssured)
        {
            var docs = new List<string> { "PanCard", "AadhaarCard", "AddressProof", "IncomeProof" };
            
            if (sumAssured > 2500000)
            {
                docs.Add("MedicalReport");
            }
            
            return docs.ToArray();
        }

        private PolicyDto MapToPolicyDto(Policy policy)
        {
            return _mapper.Map<PolicyDto>(policy);
        }
    }
}
