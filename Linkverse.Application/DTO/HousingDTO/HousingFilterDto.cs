using Linkverse.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.DTO.HousingDTO
{
    public class HousingFilterDto
    {
        public Housing? Type { get; set; }
        public ApartmentType? Apartment { get; set; }
        public Campus? Campus { get; set; }
        public string? SearchTerm { get; set; }

        [Range(0, 100_000_000)]
        public decimal? MinPrice { get; set; }

        [Range(0, 100_000_000)]
        public decimal? MaxPrice { get; set; }

        [Range(1, 100)]
        public int Page { get; set; } = 1;

        [Range(1, 50)]
        public int PageSize { get; set; } = 10;
    }
}
