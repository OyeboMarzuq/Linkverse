using Linkverse.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Domain.Entities
{
    public class Match : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid RequesterId { get; set; }
        public Guid MatchedId { get; set; }
        public TalkingStage Status { get; set; }
        public double CompatibilityScore { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public MatchProfile Requester { get; set; } = null!;
    }
}
