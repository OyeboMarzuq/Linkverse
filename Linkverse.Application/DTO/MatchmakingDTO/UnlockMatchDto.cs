using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.DTO.MatchmakingDTO
{
    public class UnlockMatchDto
    {
        [Required(ErrorMessage = "Match ID is required.")]
        public Guid MatchId { get; set; }
    }
}
