using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Domain.Entities
{
    public class Conversation
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ParticipantAId { get; set; }
        public Guid ParticipantBId { get; set; }
        public Guid? ServiceId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastMessageAt { get; set; }

        public User ParticipantA { get; set; } = null!;
        public User ParticipantB { get; set; } = null!;
        public Service? Service { get; set; }
        public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}
