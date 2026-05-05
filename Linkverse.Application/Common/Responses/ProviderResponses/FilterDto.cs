using Linkverse.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.Common.Responses.ProviderResponses
{
    public class FilterDto
    {
        public Occupation? Occupation { get; set; }
        public string? SearchTerm { get; set; }
        public bool VerifiedOnly { get; set; } = false;

        [Range(1, 100)]
        public int Page { get; set; } = 1;

        [Range(1, 50)]
        public int PageSize { get; set; } = 10;
    }
}
