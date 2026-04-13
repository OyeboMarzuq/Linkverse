using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Domain.Entities
{
    public class PDFPurchase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid NoteId { get; set; }
        public Guid UserId { get; set; }
        public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;

        public StudyPDF PDF { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
