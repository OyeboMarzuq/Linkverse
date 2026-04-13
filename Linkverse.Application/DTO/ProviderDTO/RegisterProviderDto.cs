using Linkverse.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.DTO.ProviderDTO
{
    public class RegisterProviderDto
    {
        [Required(ErrorMessage = "Provider type is required.")]
        public ProviderType Type { get; set; }

        [Required(ErrorMessage = "Business name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Business name must be between 2 and 100 characters.")]
        public string BusinessName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [StringLength(150, ErrorMessage = "Location cannot exceed 150 characters.")]
        public string? Location { get; set; }

        [StringLength(100, ErrorMessage = "License number cannot exceed 100 characters.")]
        public string? LicenseNumber { get; set; }

        [Required(ErrorMessage = "Bank details are required.")]
        public CreateBankDetailsDto BankDetails { get; set; } = null!;
    }
}
