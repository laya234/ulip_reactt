using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ULIP_proj.DTOs;
using ULIP_proj.Interfaces;

namespace ULIP_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);
            if (result == null)
                return Unauthorized("Invalid credentials");

            return Ok(new { 
                token = result.Token,
                userId = result.User.UserId,
                role = result.User.Role,
                username = $"{result.User.FirstName} {result.User.LastName}"
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto registerDto)
        {
            try
            {
                var result = await _authService.RegisterAsync(registerDto);
                if (result == null)
                    return Ok(new { success = false, message = "Email already exists" });

                return Ok(new { success = true, message = "Registration successful" });
            }
            catch (ArgumentException ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "An error occurred during registration" });
            }
        }


    }
}
