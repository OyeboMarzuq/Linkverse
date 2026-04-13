using Linkverse.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.DTO.HousingDTO
{
    public class CreateHousingListingDto
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(150, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 150 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Housing type is required.")]
        public Housing Type { get; set; }

        [Required(ErrorMessage = "Apartment type is required.")]
        public ApartmentType Apartment { get; set; }

        [Required(ErrorMessage = "Campus is required.")]
        public Campus Campus { get; set; }

        [Required(ErrorMessage = "Price per year is required.")]
        [Range(1_000, 100_000_000, ErrorMessage = "Price must be between ₦1,000 and ₦100,000,000.")]
        public decimal PricePerYear { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Location must be between 3 and 200 characters.")]
        public string Location { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1,000 characters.")]
        public string? Description { get; set; }

        [StringLength(50, ErrorMessage = "Tag cannot exceed 50 characters.")]
        public string? Tag { get; set; }

        [Required(ErrorMessage = "At least one image is required.")]
        [MinLength(1, ErrorMessage = "Please provide at least one image.")]
        [MaxLength(10, ErrorMessage = "You can upload a maximum of 10 images.")]
        public List<HousingImageDto> Images { get; set; } = new();
    }
}
