using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ULIP_proj.DTOs;
using ULIP_proj.Interfaces;

namespace ULIP_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ApprovalController : ControllerBase
    {
        private readonly IApprovalService _approvalService;

        public ApprovalController(IApprovalService approvalService)
        {
            _approvalService = approvalService;
        }

        [HttpPost("request")]
        public async Task<IActionResult> SubmitRequest([FromForm] ApprovalRequestDto requestDto)
        {
            var userId = int.Parse(User.FindFirst("userId").Value);
            var result = await _approvalService.SubmitApprovalRequestAsync(
                requestDto.RequestId,
                requestDto.RequestType,
                requestDto.Reason,
                userId);
            return Ok(new { Success = result });
        }

        [HttpGet("pending")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetPendingApprovals()
        {
            var approvals = await _approvalService.GetPendingApprovalsAsync();
            return Ok(approvals);
        }

        [HttpPost("{approvalId}/approve")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ApproveRequest(int approvalId, [FromForm] string comments)
        {
            var managerId = int.Parse(User.FindFirst("userId").Value);
            var result = await _approvalService.ApproveRequestAsync(approvalId, comments, managerId);
            return Ok(new { Success = result });
        }

        [HttpPost("{approvalId}/reject")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> RejectRequest(int approvalId, [FromForm] string comments)
        {
            var managerId = int.Parse(User.FindFirst("userId").Value);
            var result = await _approvalService.RejectRequestAsync(approvalId, comments, managerId);
            return Ok(new { Success = result });
        }
    }

    public class ApprovalRequestDto
    {
        public int RequestId { get; set; }
        public string RequestType { get; set; }
        public string Reason { get; set; }
    }
}
