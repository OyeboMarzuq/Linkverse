using Linkverse.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.DTO.HousingDTO
{
    public class HousingListingSummaryDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public Housing Type { get; set; }
        public ApartmentType? Apartment { get; set; }
        public Campus Campus { get; set; }
        public decimal PricePerYear { get; set; }
        public string Location { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? Tag { get; set; }
        public string? CoverImageUrl { get; set; }
    }
}
