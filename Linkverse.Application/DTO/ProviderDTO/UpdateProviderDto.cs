using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.DTO.ProviderDTO
{
    public class UpdateProviderDto
    {
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Business name must be between 2 and 100 characters.")]
        public string? BusinessName { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [StringLength(150, ErrorMessage = "Location cannot exceed 150 characters.")]
        public string? Location { get; set; }

        [StringLength(100, ErrorMessage = "License number cannot exceed 100 characters.")]
        public string? LicenseNumber { get; set; }
    }
}
