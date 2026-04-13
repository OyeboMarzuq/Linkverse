using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.DTO.StudyPDFDTO
{
    public class StudyPDFFilterDto
    {
        public string? SearchTerm { get; set; }

        [StringLength(20)]
        public string? CourseCode { get; set; }

        [StringLength(100)]
        public string? Department { get; set; }

        [Range(0, 50_000)]
        public decimal? MaxPrice { get; set; }

        [Range(1, 100)]
        public int Page { get; set; } = 1;

        [Range(1, 50)]
        public int PageSize { get; set; } = 10;
    }
}
