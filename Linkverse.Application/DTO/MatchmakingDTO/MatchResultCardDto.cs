using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.DTO.MatchmakingDTO
{
    public class MatchResultCardDto
    {
        public Guid MatchId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string MaskedSurname { get; set; } = string.Empty;
        public string? Gender { get; set; }
        public string? Religion { get; set; }
        public string? StateOfOrigin { get; set; }
        public string? Height { get; set; }
        public string? Qualification { get; set; }
        public string? Occupation { get; set; }
        public string? Favourite { get; set; }
        public string? Bio { get; set; }

        public bool IsUnlocked { get; set; }
        public string? Phone { get; set; }       // "080****5678" or full if unlocked
        public string? Location { get; set; }    // "●●●●●●●●" or full if unlocked
        public string? PhotoUrl { get; set; }    // null/"Hidden" or real URL if unlocked
    }
}