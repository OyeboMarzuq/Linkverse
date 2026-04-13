using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Domain.Entities
{
    public class StudyPDF : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ProviderId { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Pages { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public int DownloadCount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ServiceProviders Provider { get; set; } = null!;
        public ICollection<PDFPurchase> Purchases { get; set; } = new List<PDFPurchase>();
    }
}
