using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.DTO.BookingDTO
{
    public class BookingAgreementSummaryDto
    {
        public Guid Id { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? DocumentUrl { get; set; }
    }
}
