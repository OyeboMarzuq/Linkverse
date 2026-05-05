using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Domain.Entities
{
    public class MatchResult : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SeekerId { get; set; }
        public Guid UnlockedProfileId { get; set; }
        public string UnlockToken { get; set; } = string.Empty;
        public DateTime TokenExpiresAt { get; set; }

        public bool IsExpired => DateTime.UtcNow > TokenExpiresAt;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User Seeker { get; set; } = null!;
        public MatchProfile UnlockedProfile { get; set; } = null!;
    }
}
