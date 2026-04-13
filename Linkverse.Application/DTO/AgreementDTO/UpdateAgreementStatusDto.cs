using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.DTO.AgreementDTO
{
    public class UpdateAgreementStatusDto
    {
        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression("^(Pending|Active|Completed|Cancelled)$",
            ErrorMessage = "Status must be one of: Pending, Active, Completed, Cancelled.")]
        public string Status { get; set; } = string.Empty;
    }
}
