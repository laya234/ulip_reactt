using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ULIP_proj.DTOs;
using ULIP_proj.Interfaces;

namespace ULIP_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FundController : ControllerBase
    {
        private readonly IFundService _fundService;

        public FundController(IFundService fundService)
        {
            _fundService = fundService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableFunds()
        {
            var funds = await _fundService.GetAvailableFundsAsync();
            return Ok(funds);
        }

        [HttpPost("{fundId}/update-nav")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateNAV(int fundId, [FromForm] decimal newNAV)
        {
            var result = await _fundService.UpdateDailyNAVAsync(fundId, newNAV);
            return Ok(new { Success = result });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CreateFund([FromForm] CreateFundDto fundDto)
        {
            var result = await _fundService.CreateFundAsync(fundDto);
            return Ok(result);
        }

        [HttpGet("{fundId}/details")]
        public async Task<IActionResult> GetFundDetails(int fundId)
        {
            try
            {
                var fundDetails = await _fundService.GetFundDetailsAsync(fundId);
                if (fundDetails == null)
                    return NotFound("Fund not found");
                    
                return Ok(fundDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

    }
}
