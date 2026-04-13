using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.DTO.StudyPDFDTO
{
    public class UpdateStudyPDFDto
    {
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Course code must be between 3 and 20 characters.")]
        [RegularExpression(@"^[A-Za-z]{2,4}\s?\d{3}$",
            ErrorMessage = "Course code format must be like 'MTH 301' or 'CSC201'.")]
        public string? CourseCode { get; set; }

        [StringLength(100, ErrorMessage = "Department cannot exceed 100 characters.")]
        public string? Department { get; set; }

        [Range(0, 50_000, ErrorMessage = "Price must be between ₦0 and ₦50,000.")]
        public decimal? Price { get; set; }

        [Range(1, 2000, ErrorMessage = "Pages must be between 1 and 2,000.")]
        public int? Pages { get; set; }

        [Url(ErrorMessage = "Must be a valid file URL.")]
        public string? FileUrl { get; set; }
    }
}
