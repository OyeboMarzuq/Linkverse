using Linkverse.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.DTO.ProviderDTO
{
    public class ProviderSummaryDto
    {
        public Guid Id { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public Occupation Type { get; set; }
        public string? Location { get; set; }
        public bool IsVerified { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
    }
}
