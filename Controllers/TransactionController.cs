using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ULIP_proj.DTOs;
using ULIP_proj.Interfaces;

namespace ULIP_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("invest")]
        public async Task<IActionResult> ProcessInvestment([FromForm] InvestmentDto investmentDto)
        {
            try
            {
                var result = await _transactionService.ProcessInvestmentAsync(
                    investmentDto.PolicyId, 
                    investmentDto.Amount, 
                    investmentDto.FundId);
                
                if (result == null)
                    return Ok(new { Success = false, Error = "Investment processing failed - result is null" });
                    
                return Ok(new { Success = true, Data = result });
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Error = ex.Message });
            }
        }

        [HttpPost("fund-switch")]
        public async Task<IActionResult> ProcessFundSwitch([FromForm] FundSwitchDto switchDto)
        {
            try
            {

                
                var result = await _transactionService.ProcessFundSwitchAsync(
                    switchDto.PolicyId,
                    switchDto.FromFundId,
                    switchDto.ToFundId,
                    switchDto.Amount);
                return Ok(new { Success = result });
            }
            catch (Exception ex)
            {

                return Ok(new { Success = false, Error = ex.Message });
            }
        }

        [HttpPost("{transactionId}/request-approval")]
        public async Task<IActionResult> RequestApproval(int transactionId)
        {
            var result = await _transactionService.RequestHighValueApprovalAsync(transactionId);
            return Ok(new { Success = result });
        }

        [HttpGet("policy/{policyId}/current-value")]
        public async Task<IActionResult> GetPolicyCurrentValue(int policyId)
        {
            var value = await _transactionService.GetPolicyCurrentValueAsync(policyId);
            return Ok(new { CurrentValue = value });
        }

        [HttpGet("by-policy/{policyId}/transactions")]
        public async Task<IActionResult> GetTransactionsByPolicy(int policyId)
        {
            try
            {
                var transactions = await _transactionService.GetTransactionsByPolicyAsync(policyId);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return Ok(new { Success = false, Error = ex.Message });
            }
        }

        [HttpGet("policy/{policyId}/portfolio-value")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetPortfolioValue(int policyId)
        {
            try
            {
                var portfolioValue = await _transactionService.CalculatePortfolioValueAsync(policyId);
                return Ok(portfolioValue);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }
    }

    public class InvestmentDto
    {
        public int PolicyId { get; set; }
        public decimal Amount { get; set; }
        public int FundId { get; set; }
    }

    public class FundSwitchDto
    {
        public int PolicyId { get; set; }
        public int FromFundId { get; set; }
        public int ToFundId { get; set; }
        public decimal Amount { get; set; }
    }
}
