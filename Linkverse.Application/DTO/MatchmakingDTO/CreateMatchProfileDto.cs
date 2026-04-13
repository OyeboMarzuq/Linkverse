using Linkverse.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.DTO.MatchmakingDTO
{
    public class CreateMatchProfileDto
    {
        [Required(ErrorMessage = "Relationship type is required.")]
        public RelationshipType LookingFor { get; set; }

        [StringLength(50, ErrorMessage = "Religion preference cannot exceed 50 characters.")]
        public string? ReligionPreference { get; set; }

        [StringLength(30, ErrorMessage = "Height preference cannot exceed 30 characters.")]
        public string? HeightPreference { get; set; }

        [Range(16, 60, ErrorMessage = "Minimum age must be between 16 and 60.")]
        public int? MinAge { get; set; }

        [Range(16, 60, ErrorMessage = "Maximum age must be between 16 and 60.")]
        public int? MaxAge { get; set; }

        [StringLength(100, ErrorMessage = "Department cannot exceed 100 characters.")]
        public string? Department { get; set; }

        [StringLength(300, ErrorMessage = "Bio cannot exceed 300 characters.")]
        public string? Bio { get; set; }
    }
}
