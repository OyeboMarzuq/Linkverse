using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.Common.Responses.StudyPDFResponse
{
    public class StudyPDFResponseDto
    {
        public Guid Id { get; set; }
        public Guid ProviderId { get; set; }
        public string ProviderName { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Pages { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public int DownloadCount { get; set; }
        public DateTime CreatedAt { get; set; }

        /// <summary>True if the currently authenticated user has already purchased this PDF.</summary>
        public bool AlreadyPurchased { get; set; }
    }
}
