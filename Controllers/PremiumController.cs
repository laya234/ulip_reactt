using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ULIP_proj.DTOs;
using ULIP_proj.Interfaces;
using ULIP_proj.Enums;

namespace ULIP_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PremiumController : ControllerBase
    {
        private readonly IPremiumService _premiumService;

        public PremiumController(IPremiumService premiumService)
        {
            _premiumService = premiumService;
        }

        [HttpPost("pay")]
        public async Task<IActionResult> PayPremium()
        {
            Console.WriteLine($"[CONTROLLER] Raw form data:");
            foreach (var key in Request.Form.Keys)
            {
                Console.WriteLine($"[CONTROLLER] {key}: {Request.Form[key]}");
            }
            
            var policyId = int.Parse(Request.Form["PolicyId"]);
            var fundId = int.Parse(Request.Form["FundId"]);
            var premiumAmount = decimal.Parse(Request.Form["PremiumAmount"]);
            var dueDate = DateTime.Parse(Request.Form["DueDate"]);
            var paymentMethod = Enum.Parse<PaymentMethod>(Request.Form["PaymentMethod"]);
            
            Console.WriteLine($"[CONTROLLER] Parsed - Policy: {policyId}, Amount: {premiumAmount}, Fund: {fundId}");
            
            try
            {
                var premiumDto = new PayPremiumDto
                {
                    PolicyId = policyId,
                    FundId = fundId,
                    PremiumAmount = premiumAmount,
                    DueDate = dueDate,
                    PaymentMethod = paymentMethod
                };
                var result = await _premiumService.ProcessPremiumPaymentAsync(premiumDto);
                Console.WriteLine($"[CONTROLLER] Premium service returned: {result}");
                return Ok(new { Success = result });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CONTROLLER] Premium payment failed: {ex.Message}");
                return Ok(new { Success = false, Error = ex.Message });
            }
        }

        [HttpGet("overdue")]
        [Authorize(Roles = "Agent,Manager")]
        public async Task<IActionResult> GetOverduePremiums()
        {
            var overdue = await _premiumService.GetOverduePremumsAsync();
            return Ok(overdue);
        }

        [HttpPost("{policyId}/send-reminder")]
        [Authorize(Roles = "Agent,Manager")]
        public async Task<IActionResult> SendReminder(int policyId)
        {
            var result = await _premiumService.SendReminderAsync(policyId);
            return Ok(new { Success = result });
        }

        [HttpGet("{policyId}/calculate-commission")]
        [Authorize(Roles = "Agent")]
        public async Task<IActionResult> CalculateCommission(int policyId, [FromQuery] decimal premiumAmount)
        {
            var commission = await _premiumService.CalculateCommissionAsync(policyId, premiumAmount);
            return Ok(new { Commission = commission });
        }

        [HttpGet("policy/{policyId}")]
        public async Task<IActionResult> GetPremiumSchedule(int policyId)
        {
            var schedule = await _premiumService.GetPremiumScheduleAsync(policyId);
            return Ok(schedule);
        }

        [HttpPost("process-overdue")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ProcessOverduePolicies()
        {
            var result = await _premiumService.ProcessOverduePoliciesAsync();
            return Ok(new { Success = result });
        }
    }
}
