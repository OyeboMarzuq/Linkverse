using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.Common.Responses.AgreementResponse
{
    public class AgreementResponseDto
    {
        public Guid Id { get; set; }

        /// <summary>Auto-generated reference e.g. "LV-2026-00847"</summary>
        public string ReferenceNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        /// <summary>SHA-256 hash used to verify document has not been tampered with.</summary>
        public string? DigitalStampHash { get; set; }

        public string? DocumentUrl { get; set; }
        public Guid PartyAId { get; set; }
        public string PartyAName { get; set; } = string.Empty;
        public Guid PartyBId { get; set; }
        public string PartyBName { get; set; } = string.Empty;
        public Guid? BookingId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? SignedAt { get; set; }
    }
}
