using Linkverse.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.DTO.MatchmakingDTO
{
    public class UnlockedMatchProfileDto
    {
        public Guid MatchProfileId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public Gender? Gender { get; set; }
        public string? Religion { get; set; }
        public string? StateOfOrigin { get; set; }
        public string? Height { get; set; }
        public string? Qualification { get; set; }
        public string? Occupation { get; set; }
        public string? Favourite { get; set; }
        public string? Bio { get; set; }
        public string? Phone { get; set; }
        public string? Location { get; set; }
        public string? PhotoUrl { get; set; }
        public DateTime TokenExpiresAt { get; set; }
    }
}
