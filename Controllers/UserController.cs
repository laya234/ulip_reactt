using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ULIP_proj.DTOs;
using ULIP_proj.Interfaces;

namespace ULIP_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = int.Parse(User.FindFirst("userId").Value);
            var profile = await _userService.GetUserProfileAsync(userId);
            return Ok(profile);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileDto updateDto)
        {
            var userId = int.Parse(User.FindFirst("userId").Value);
            var result = await _userService.UpdateProfileAsync(updateDto, userId);
            return Ok(result);
        }

        [HttpGet("commission")]
        [Authorize(Roles = "Agent")]
        public async Task<IActionResult> GetCommission()
        {
            var userId = int.Parse(User.FindFirst("userId").Value);
            var commission = await _userService.GetAgentCommissionAsync(userId);
            return Ok(new { TotalCommission = commission });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("managers")]
        public async Task<IActionResult> GetManagers()
        {
            var managers = await _userService.GetUsersByRoleAsync("Manager");
            return Ok(managers);
        }

        [HttpPost("upload-document")]
        public async Task<IActionResult> UploadDocument(IFormFile file, [FromForm] string documentType)
        {
            var userId = int.Parse(User.FindFirst("userId").Value);
            var result = await _userService.UploadDocumentAsync(userId, file, documentType);
            return Ok(new { Success = result });
        }

        [HttpGet("documents")]
        public async Task<IActionResult> GetDocuments()
        {
            var userId = int.Parse(User.FindFirst("userId").Value);
            var documents = await _userService.GetUserDocumentsAsync(userId);
            return Ok(documents);
        }

        [HttpGet("download-document/{documentId}")]
        public async Task<IActionResult> DownloadDocument(int documentId)
        {
            var userId = int.Parse(User.FindFirst("userId").Value);
            var fileBytes = await _userService.DownloadDocumentAsync(documentId, userId);
            
            if (fileBytes == null)
                return NotFound();
            
            return File(fileBytes, "application/octet-stream", $"document_{documentId}.pdf");
        }

        [HttpPost("customer-lookup")]
        [Authorize(Roles = "Agent,Manager,Admin")]
        public async Task<IActionResult> CustomerLookup([FromBody] FindCustomerRequest request)
        {
            var user = await _userService.GetUserByPhoneAsync(request.PhoneNumber);
            if (user == null)
                return NotFound("Customer not found with this phone number");
            
            return Ok(user);
        }
    }
}
