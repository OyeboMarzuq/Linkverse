using Linkverse.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.Common.Responses.MatchmakingResponse
{
    public class MatchProfileResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public RelationshipType LookingFor { get; set; }
        public string? ReligionPreference { get; set; }
        public string? HeightPreference { get; set; }
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public string? Department { get; set; }
        public string? Bio { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
