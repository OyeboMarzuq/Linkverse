using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.DTO.StudyPDFDTO
{
    public class StudyPDFSummaryDto
    {
        public Guid Id { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Pages { get; set; }
        public int DownloadCount { get; set; }
        public string ProviderName { get; set; } = string.Empty;
    }
}
