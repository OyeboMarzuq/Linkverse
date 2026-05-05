using Linkverse.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Linkverse.Domain.Entities
{
    public class MatchProfile
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public RelationshipType LookingFor { get; set; }
        public string? ReligionPreference { get; set; }
        public string? HeightPreference { get; set; }
        public int? MinAge { get; set; }
        public Gender? Gender { get; set; }
        public int? MaxAge { get; set; }
        public string? Department { get; set; }
        public string? Bio { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
        public ICollection<Match> MatchesAsRequester { get; set; } = new List<Match>();
    }
}
