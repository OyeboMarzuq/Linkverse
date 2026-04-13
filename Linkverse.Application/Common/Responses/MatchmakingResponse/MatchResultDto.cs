using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.Common.Responses.MatchmakingResponse
{
    public class MatchResultDto
    {
        public Guid MatchProfileId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string MaskedLastName { get; set; } = string.Empty;

        public string? Department { get; set; }
        public string? ReligionPreference { get; set; }
        public string? HeightPreference { get; set; }
        public string? Bio { get; set; }
        public double CompatibilityScore { get; set; }
        public bool IsUnlocked { get; set; }
        public string? FullName { get; set; }
        public string? MaskedPhone { get; set; }

        public string? Location { get; set; }
    }
}
