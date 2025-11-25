using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ULIP_proj.DTOs;
using ULIP_proj.Interfaces;

namespace ULIP_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PolicyController : ControllerBase
    {
        private readonly IPolicyService _policyService;
        private readonly IFileService _fileService;
        private readonly ITransactionService _transactionService;
        private readonly IFundService _fundService;

        public PolicyController(IPolicyService policyService, IFileService fileService, ITransactionService transactionService, IFundService fundService)
        {
            _policyService = policyService;
            _fileService = fileService;
            _transactionService = transactionService;
            _fundService = fundService;
        }

        [HttpPost]
        [Authorize(Roles = "Agent")]
        public async Task<IActionResult> CreatePolicy([FromForm] CreatePolicyDto policyDto)
        {
            var agentId = int.Parse(User.FindFirst("userId").Value);
            var result = await _policyService.CreatePolicyAsync(policyDto, agentId);
            return Ok(result);
        }

        [HttpPost("details")]
        public async Task<IActionResult> GetPolicy([FromBody] PolicyDetailsRequest request)
        {
            var policy = await _policyService.GetPolicyDetailsAsync(request.PolicyId);
            return Ok(policy);
        }

        [HttpGet("my-policies")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyPolicies()
        {
            var userId = int.Parse(User.FindFirst("userId").Value);
            var policies = await _policyService.GetMyPoliciesAsync(userId);
            return Ok(policies);
        }

        [HttpGet("agent-policies")]
        [Authorize(Roles = "Agent")]
        public async Task<IActionResult> GetAgentPolicies()
        {
            var agentId = int.Parse(User.FindFirst("userId").Value);
            var policies = await _policyService.GetAgentPoliciesAsync(agentId);
            return Ok(policies);
        }

        [HttpPost("surrender")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> RequestSurrender([FromBody] SurrenderRequest request)
        {
            var userId = int.Parse(User.FindFirst("userId").Value);
            var result = await _policyService.RequestSurrenderAsync(request.PolicyId, request.Reason, userId);
            return Ok(new { success = result });
        }

        [HttpPost("surrender-value")]
        public async Task<IActionResult> GetSurrenderValue([FromBody] PolicyDetailsRequest request)
        {
            var value = await _policyService.CalculateSurrenderValueAsync(request.PolicyId);
            return Ok(new { SurrenderValue = value });
        }

        [HttpPost("{policyId}/upload-document")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UploadDocument(int policyId, IFormFile file, [FromQuery] string documentType)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("userId").Value);
                var result = await _policyService.UploadPolicyDocumentAsync(policyId, file, documentType, userId);
                return Ok(new { Success = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpPost("{policyId}/verify-documents")]
        [Authorize(Roles = "Agent,Manager")]
        public async Task<IActionResult> VerifyDocuments(int policyId, [FromQuery] string documentType, [FromQuery] bool approved, [FromQuery] string? comments = null)
        {
            try
            {
                var verifierId = int.Parse(User.FindFirst("userId").Value);
                var result = await _policyService.VerifyDocumentAsync(policyId, documentType, approved, verifierId, comments);
                return Ok(new { Success = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpGet("{policyId}/document-status")]
        [Authorize(Roles = "Customer,Agent,Manager")]
        public async Task<IActionResult> GetDocumentStatus(int policyId)
        {
            try
            {
                var status = await _policyService.GetDocumentStatusAsync(policyId);
                return Ok(status);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpGet("pending-verification")]
        [Authorize(Roles = "Agent,Manager")]
        public async Task<IActionResult> GetPoliciesWithPendingDocuments()
        {
            try
            {
                var policies = await _policyService.GetPoliciesWithPendingDocumentsAsync();
                return Ok(policies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpGet("{policyId}/download-document/{documentType}")]
        [Authorize(Roles = "Agent,Manager,Customer")]
        public async Task<IActionResult> DownloadDocument(int policyId, string documentType)
        {
            try
            {
                var testContent = $"Test document for Policy {policyId} - {documentType}\nUploaded on: {DateTime.Now}";
                var testBytes = System.Text.Encoding.UTF8.GetBytes(testContent);
                
                return File(testBytes, "text/plain", $"{documentType}_{policyId}.txt");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpPost("proposal/{saleId}")]
        [Authorize(Roles = "Agent")]
        public async Task<IActionResult> CreatePolicyProposal(int saleId, [FromForm] CreatePolicyProposalDto proposalDto)
        {
            var agentId = int.Parse(User.FindFirst("userId").Value);
            var result = await _policyService.CreatePolicyProposalAsync(saleId, proposalDto, agentId);
            return Ok(result);
        }

        [HttpGet("pending-proposals")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetPendingProposals()
        {
            var userId = int.Parse(User.FindFirst("userId").Value);
            var proposals = await _policyService.GetPendingProposalsAsync(userId);
            return Ok(proposals);
        }

        [HttpPost("accept")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AcceptPolicyProposal([FromBody] AcceptProposalRequest request)
        {
            var acceptanceDto = new PolicyAcceptanceDto
            {
                Accepted = request.Accepted,
                RequireDocuments = request.RequireDocuments ?? false
            };
            var result = await _policyService.AcceptPolicyProposalAsync(request.PolicyId, acceptanceDto);
            return Ok(new { Success = result });
        }

        [HttpPost("create-proposal")]
        [Authorize(Roles = "Agent")]
        public async Task<IActionResult> CreatePolicyProposalForCustomer([FromForm] CreatePolicyProposalWithCustomerDto proposalDto)
        {
            var agentId = int.Parse(User.FindFirst("userId").Value);
            var result = await _policyService.CreatePolicyProposalForCustomerAsync(proposalDto.CustomerId, proposalDto, agentId);
            return Ok(result);
        }

        [HttpPost("{policyId}/generate-statement")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GenerateStatement(int policyId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("userId").Value);
                var pdfBytes = await _policyService.GeneratePolicyStatementAsync(policyId, userId);
                
                if (pdfBytes == null)
                    return NotFound("Policy not found or access denied");
                
                return File(pdfBytes, "application/pdf", $"Policy_Statement_{policyId}.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpGet("{policyId}/portfolio")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetPolicyPortfolio(int policyId)
        {
            var userId = int.Parse(User.FindFirst("userId").Value);
            var policy = await _policyService.GetPolicyDetailsAsync(policyId);
            
            if (policy == null)
                return NotFound("Policy not found");
                
            if (policy.UserId != userId)
                return Forbid("Access denied");
            
            return Ok(policy);
        }

        [HttpPost("{policyId}/approve-surrender")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ApproveSurrender(int policyId, [FromQuery] bool approved, [FromQuery] string? comments = null)
        {
            try
            {
                var managerId = int.Parse(User.FindFirst("userId").Value);
                var result = await _policyService.ProcessSurrenderApprovalAsync(policyId, approved, managerId, comments);
                return Ok(new { Success = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpPost("complete-details")]
        public async Task<IActionResult> GetCompleteDetails([FromBody] PolicyDetailsRequest request)
        {
            try
            {
                var policy = await _policyService.GetPolicyDetailsAsync(request.PolicyId);
                if (policy == null)
                    return NotFound("Policy not found");

                var surrenderValue = await _policyService.CalculateSurrenderValueAsync(request.PolicyId);

                var transactions = await _transactionService.GetTransactionsByPolicyAsync(request.PolicyId);

                var funds = await _fundService.GetAvailableFundsAsync();
                var fundsList = funds.ToList();
                Console.WriteLine($"[COMPLETE-DETAILS] Fetched {fundsList.Count} funds");

                return Ok(new {
                    Policy = policy,
                    SurrenderValue = surrenderValue,
                    Transactions = transactions,
                    Funds = fundsList
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

    public class CreatePolicyProposalWithCustomerDto : CreatePolicyProposalDto
    {
        public int CustomerId { get; set; }
    }


    }
}
