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
        [Required(ErrorMessage = "Match profile ID is required.")]
        public Guid MatchId { get; set; }

        [Required(ErrorMessage = "Unlock token is required.")]
        public string UnlockToken { get; set; } = string.Empty;
    }
}
