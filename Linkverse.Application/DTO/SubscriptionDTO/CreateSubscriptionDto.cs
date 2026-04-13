using Linkverse.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.DTO.SubscriptionDTO
{
    public class CreateSubscriptionDto
    {
        [Required(ErrorMessage = "Subscription plan is required.")]
        public SubscriptionPlan Plan { get; set; }
    }
}
