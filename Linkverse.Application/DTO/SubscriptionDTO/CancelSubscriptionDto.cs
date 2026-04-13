using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.DTO.SubscriptionDTO
{
    public class CancelSubscriptionDto
    {
        [Required(ErrorMessage = "Subscription ID is required.")]
        public Guid SubscriptionId { get; set; }
    }   
}
