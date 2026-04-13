using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.DTO.HousingDTO
{
    public class HousingImageDto
    {
        [Required(ErrorMessage = "Image URL is required.")]
        [Url(ErrorMessage = "Must be a valid image URL.")]
        public string ImageUrl { get; set; } = string.Empty;

        [Range(0, 9, ErrorMessage = "Sort order must be between 0 and 9.")]
        public int SortOrder { get; set; }
    }
}
