using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.DTO.UserProfileDTO
{
    public class CreateUserProfileDto
    {
        [StringLength(300, ErrorMessage = "Bio cannot exceed 300 characters.")]
        public string? Bio { get; set; }

        [StringLength(100, ErrorMessage = "Department cannot exceed 100 characters.")]
        public string? Department { get; set; }

        [StringLength(100, ErrorMessage = "Faculty cannot exceed 100 characters.")]
        public string? Faculty { get; set; }

        [Range(100, 700, ErrorMessage = "Level must be between 100 and 700.")]
        public int? Level { get; set; }

        [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters.")]
        public string? Location { get; set; }
    }
}