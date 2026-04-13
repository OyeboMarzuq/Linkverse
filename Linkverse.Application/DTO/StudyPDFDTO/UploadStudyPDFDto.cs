using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.DTO.StudyPDFDTO
{
    public class UploadStudyPDFDto
    {
        [Required(ErrorMessage = "Course code is required.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Course code must be between 3 and 20 characters.")]
        [RegularExpression(@"^[A-Za-z]{2,4}\s?\d{3}$",
            ErrorMessage = "Course code format must be like 'MTH 301' or 'CSC201'.")]
        public string CourseCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Department is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Department must be between 2 and 100 characters.")]
        public string Department { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required.")]
        [Range(0, 50_000, ErrorMessage = "Price must be between ₦0 and ₦50,000.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Page count is required.")]
        [Range(1, 2000, ErrorMessage = "Pages must be between 1 and 2,000.")]
        public int Pages { get; set; }

        [Required(ErrorMessage = "File URL is required.")]
        [Url(ErrorMessage = "Must be a valid file URL.")]
        public string FileUrl { get; set; } = string.Empty;
    }
}
