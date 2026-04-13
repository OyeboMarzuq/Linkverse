using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.DTO.AgreementDTO
{
    public class AgreementSummaryDto
    {
        public Guid Id { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        /// <summary>e.g. "John Doe ↔ King's Barber Studio"</summary>
        public string Parties { get; set; } = string.Empty;

        public string? DocumentUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
