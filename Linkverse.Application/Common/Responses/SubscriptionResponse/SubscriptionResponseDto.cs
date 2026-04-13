using Linkverse.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.Common.Responses.SubscriptionResponse
{
    public class SubscriptionResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public SubscriptionPlan Plan { get; set; }
        public decimal PricePerMonth { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        /// <summary>Null if subscription has no expiry (e.g. Intermediate/free tier).</summary>
        public int? DaysRemaining { get; set; }
    }
}
