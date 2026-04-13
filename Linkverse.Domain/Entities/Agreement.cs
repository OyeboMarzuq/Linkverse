using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Domain.Entities
{
    public class Agreement : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ReferenceNumber { get; set; } = string.Empty; // e.g. "LV-2026-00847"
        public Guid PartyAId { get; set; }
        public Guid PartyBId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty; // Generated agreement text
        public string Status { get; set; } = "Pending"; // Pending, Active, Completed, Cancelled
        public string? DigitalStampHash { get; set; } // SHA-256 hash for tamper-proofing
        public string? DocumentUrl { get; set; } // PDF download link
        public Guid? BookingId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? SignedAt { get; set; }

        public User PartyA { get; set; } = null!;
        public Booking? Booking { get; set; }
    }
}
