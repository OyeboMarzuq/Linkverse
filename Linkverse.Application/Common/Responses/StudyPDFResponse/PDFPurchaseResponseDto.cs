using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.Common.Responses.StudyPDFResponse
{
    public class PDFPurchaseResponseDto
    {
        public Guid Id { get; set; }
        public Guid NoteId { get; set; }

        public string CourseCode { get; set; } = string.Empty;

        /// <summary>Download URL — only returned after successful purchase.</summary>
        public string FileUrl { get; set; } = string.Empty;

        public DateTime PurchasedAt { get; set; }
    }
}
