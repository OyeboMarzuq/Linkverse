using Linkverse.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Domain.Entities
{
    public class HousingListing : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ProviderId { get; set; }
        public string Title { get; set; } = string.Empty;
        public Housing Type { get; set; }
        public decimal PricePerYear { get; set; }
        public string Location { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Tag { get; set; } // Popular, New, Verified, Premium

        public ServiceProviders Provider { get; set; } = null!;
        public ApartmentType? Apartment { get; set; }
        public ICollection<HousingImage> Images { get; set; } = new List<HousingImage>();
    }
}
