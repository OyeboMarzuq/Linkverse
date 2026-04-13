using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.DTO.StudyPDFDTO
{
    public class PurchasePDFDto
    {
        [Required(ErrorMessage = "PDF ID is required.")]
        public Guid NoteId { get; set; }
    }
}
