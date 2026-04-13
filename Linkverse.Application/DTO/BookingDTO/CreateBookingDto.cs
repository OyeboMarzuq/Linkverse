using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.DTO.BookingDTO
{
    public class CreateBookingDto
    {
        [Required(ErrorMessage = "Service ID is required.")]
        public Guid ServiceId { get; set; }

        [Required(ErrorMessage = "Booking date is required.")]
        [DataType(DataType.DateTime)]
        public DateTime BookingDate { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        public string? Notes { get; set; }
    }
}
