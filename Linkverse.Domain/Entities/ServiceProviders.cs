using Linkverse.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Domain.Entities
{
    public class ServiceProviders : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Occupation Type { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? LicenseNumber { get; set; }
        public bool IsVerified { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Bank Details
        public BankDetails? BankDetails { get; set; }
        public User User { get; set; } = null!;
        public ICollection<HousingListing> HousingListings { get; set; } = new List<HousingListing>();
        public ICollection<StudyPDF> StudyNotes { get; set; } = new List<StudyPDF>();
    }
}
