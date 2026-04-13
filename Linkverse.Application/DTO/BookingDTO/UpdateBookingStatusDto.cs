using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.DTO.BookingDTO
{
    public class UpdateBookingStatusDto
    {
        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression("^(Pending|Confirmed|Completed|Cancelled)$",
            ErrorMessage = "Status must be one of: Pending, Confirmed, Completed, Cancelled.")]
        public string Status { get; set; } = string.Empty;
    }
}
