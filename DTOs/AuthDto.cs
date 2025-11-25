using System.ComponentModel.DataAnnotations;
using ULIP_proj.Enums;

namespace ULIP_proj.DTOs
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string Password { get; set; }
    }

    public class RegisterDto
    {
        [Required]
        public string FirstName { get; set; }
        
        [Required]
        public string LastName { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
        
        [Required]
        public string PhoneNumber { get; set; }
        
        [Required]
        public DateTime DateOfBirth { get; set; }
        
        [Required]
        public string Address { get; set; }
        
        [Required]
        public string PanNumber { get; set; }
        
        [Required]
        public UserRole Role { get; set; }
    }

    public class AuthResponseDto
    {
        public string Token { get; set; }
        public UserDto User { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public class TokenClaimsDto
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
