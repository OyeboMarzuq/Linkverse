using Linkverse.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.DTO.HousingDTO
{
    public class UpdateHousingListingDto
    {
        [StringLength(150, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 150 characters.")]
        public string? Title { get; set; }
        public Housing? Type { get; set; }
        public ApartmentType? Apartment { get; set; }

        public Campus? Campus { get; set; }

        [Range(1_000, 100_000_000, ErrorMessage = "Price must be between ₦1,000 and ₦100,000,000.")]
        public decimal? PricePerYear { get; set; }

        [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters.")]
        public string? Location { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1,000 characters.")]
        public string? Description { get; set; }

        [StringLength(50, ErrorMessage = "Tag cannot exceed 50 characters.")]
        public string? Tag { get; set; }

        public bool? IsActive { get; set; }
    }
}
