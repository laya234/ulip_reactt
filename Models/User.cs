using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ULIP_proj.Enums;

namespace ULIP_proj.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        [StringLength(15)]
        public string PhoneNumber { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(500)]
        public string Address { get; set; }

        [Required]
        [StringLength(20)]
        public string PanNumber { get; set; }

        [Required]
        public UserRole Role { get; set; } = UserRole.Customer;

        [Required]
        [StringLength(255)]
        public string Password { get; set; }

        public bool IsActive { get; set; } = true;


        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalCommissionEarned { get; set; } = 0;

        public int PoliciesSold { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;


        public ICollection<Policy> Policies { get; set; } = new List<Policy>();
        public ICollection<Approval> RequestedApprovals { get; set; } = new List<Approval>();
        public ICollection<Approval> ApprovedApprovals { get; set; } = new List<Approval>();
        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }
}
