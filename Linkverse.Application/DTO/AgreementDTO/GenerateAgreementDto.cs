using Linkverse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.DTO.AgreementDTO
{
    public class GenerateAgreementDto
    {
        [Required(ErrorMessage = "Party B user ID is required.")]
        public Guid PartyBId { get; set; }

        [Required(ErrorMessage = "Agreement title is required.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 200 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Agreement content is required.")]
        [StringLength(10_000, MinimumLength = 20, ErrorMessage = "Content must be between 20 and 10,000 characters.")]
        public string Content { get; set; } = string.Empty;

        /// <summary>Optional — links this agreement to an existing booking.</summary>
        public Booking Booking { get; set; }
    }
}
