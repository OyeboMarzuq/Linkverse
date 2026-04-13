using Linkverse.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.Common.Responses.SubscriptionResponse
{
    public class SubscriptionPlanInfoDto
    {
        public SubscriptionPlan Plan { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal PricePerMonth { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<string> Features { get; set; } = new();
        public bool IsPopular { get; set; }
    }
}
