using Linkverse.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.DTO.MatchmakingDTO
{
    public class MatchSearchDto
    {
        [Required(ErrorMessage = "Relationship type is required to search.")]
        public RelationshipType LookingFor { get; set; }
        public string ReligionPreference { get; set; }
        public string? HeightPreference { get; set; }
        public string? Department { get; set; }

        [Range(16, 60)]
        public int? MinAge { get; set; }

        [Range(16, 60)]
        public int? MaxAge { get; set; }

        [Range(1, 100)]
        public int Page { get; set; } = 1;

        [Range(1, 50)]
        public int PageSize { get; set; } = 10;
    }
}
