using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.DTO.ProviderDTO
{
    public class CreateBankDetailsDto
    {
        [Required(ErrorMessage = "Bank name is required.")]
        [StringLength(100, ErrorMessage = "Bank name cannot exceed 100 characters.")]
        public string BankName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Account number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Account number must be exactly 10 digits.")]
        public string AccountNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Account name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Account name must be between 2 and 100 characters.")]
        public string AccountName { get; set; } = string.Empty;
    }
}
