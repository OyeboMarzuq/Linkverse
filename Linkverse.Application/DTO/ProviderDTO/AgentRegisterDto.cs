using Linkverse.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.DTO.ProviderDTO
{
    public class AgentRegisterDto
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(100, MinimumLength = 2)]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, MinimumLength = 2)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Business name is required.")]
        [StringLength(100, MinimumLength = 2)]
        public string BusinessName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Occupation is required.")]
        public Occupation Occupation { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(150)]
        public string? Location { get; set; }
        [Required(ErrorMessage = "Bank name is required.")]
        [StringLength(100)]
        public string BankName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Account number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Account number must be exactly 10 digits.")]
        public string AccountNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Account name is required.")]
        [StringLength(100, MinimumLength = 2)]
        public string AccountName { get; set; } = string.Empty;
    }
}
