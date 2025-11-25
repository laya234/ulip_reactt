using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ULIP_proj.DTOs;
using ULIP_proj.Interfaces;

namespace ULIP_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Agent")]
    public class SaleController : ControllerBase
    {
        private readonly ISaleService _saleService;
        private readonly IUserService _userService;

        public SaleController(ISaleService saleService, IUserService userService)
        {
            _saleService = saleService;
            _userService = userService;
        }

        [HttpPost("lead")]
        public async Task<IActionResult> CreateLead([FromForm] CreateSaleDto saleDto)
        {
            var agentId = int.Parse(User.FindFirst("userId").Value);
            var result = await _saleService.CreateLeadAsync(saleDto, agentId);
            return Ok(result);
        }

        [HttpPost("{saleId}/convert")]
        public async Task<IActionResult> ConvertLead(int saleId, [FromForm] CreatePolicyDto policyDto)
        {
            var agentId = int.Parse(User.FindFirst("userId").Value);
            var result = await _saleService.ConvertLeadToPolicyAsync(saleId, policyDto, agentId);
            return Ok(new { Success = result });
        }

        [HttpGet("my-pipeline")]
        public async Task<IActionResult> GetMyPipeline()
        {
            var agentId = int.Parse(User.FindFirst("userId").Value);
            var pipeline = await _saleService.GetMyPipelineAsync(agentId);
            return Ok(pipeline);
        }

        [HttpGet("conversion-rate")]
        public async Task<IActionResult> GetConversionRate()
        {
            var agentId = int.Parse(User.FindFirst("userId").Value);
            var rate = await _saleService.GetMyConversionRateAsync(agentId);
            return Ok(new { ConversionRate = rate });
        }

        [HttpGet("dashboard-data")]
        public async Task<IActionResult> GetDashboardData()
        {
            var agentId = int.Parse(User.FindFirst("userId").Value);
            var pipeline = await _saleService.GetMyPipelineAsync(agentId);
            var conversionRate = await _saleService.GetMyConversionRateAsync(agentId);
            var commission = await _userService.GetAgentCommissionAsync(agentId);
            
            return Ok(new {
                Pipeline = pipeline,
                ConversionRate = conversionRate,
                TotalCommission = commission
            });
        }

        [HttpGet("debug-sales")]
        public async Task<IActionResult> DebugSales()
        {
            var agentId = int.Parse(User.FindFirst("userId").Value);
            var pipeline = await _saleService.GetMyPipelineAsync(agentId);
            return Ok(pipeline.Select(s => new { 
                Id = s.Id, 
                CustomerName = s.CustomerName, 
                Status = s.Status.ToString(), 
                PolicyId = s.PolicyId,
                QuotedAmount = s.QuotedAmount
            }));
        }
    }
}
