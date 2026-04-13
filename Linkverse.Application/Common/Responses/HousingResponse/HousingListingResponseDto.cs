using Linkverse.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.Common.Responses.HousingResponse
{
    public class HousingListingResponseDto
    {
        public Guid Id { get; set; }
        public Guid ProviderId { get; set; }
        public string ProviderName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public Housing Type { get; set; }
        public ApartmentType? Apartment { get; set; }
        public Campus Campus { get; set; }
        public decimal PricePerYear { get; set; }
        public string Location { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public string? Tag { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<HousingImageResponseDto> Images { get; set; } = new();
    }
}
