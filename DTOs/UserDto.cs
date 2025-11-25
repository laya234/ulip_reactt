using System.ComponentModel.DataAnnotations;
using ULIP_proj.Enums;

namespace ULIP_proj.DTOs
{
    public class CreateUserDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string PanNumber { get; set; }
    }

    public class UpdateProfileDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
    }

    public class FindCustomerRequest
    {
        [Required]
        public string PhoneNumber { get; set; }
    }

    public class UserDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public string PanNumber { get; set; }
        public UserRole Role { get; set; }
        public decimal TotalCommissionEarned { get; set; }
        public int PoliciesSold { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
