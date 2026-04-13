using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Domain.Entities
{
    public class HousingImage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ListingId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public HousingListing Listing { get; set; } = null!;
    }
}
