using Linkverse.Application.DTO.BookingDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.Common.Responses.BookingResponse
{
    public class BookingResponseDto
    {
        public Guid Id { get; set; }
        public Guid ServiceId { get; set; }
        public Guid UserId { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        /// <summary>Populated if an agreement has been generated for this booking.</summary>
        public BookingAgreementSummaryDto? Agreement { get; set; }
    }
}
