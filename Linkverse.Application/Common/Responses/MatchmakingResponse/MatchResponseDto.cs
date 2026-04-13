using Linkverse.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.Common.Responses.MatchmakingResponse
{
    public class MatchResponseDto
    {
        public Guid Id { get; set; }
        public Guid RequesterId { get; set; }
        public Guid MatchedId { get; set; }
        public TalkingStage Status { get; set; }
        public double CompatibilityScore { get; set; }
        public DateTime CreatedAt { get; set; }
        public MatchResultDto MatchedProfile { get; set; } = null!;
    }
}
