using Linkverse.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.DTO.MatchmakingDTO
{
    public class UpdateMatchStatusDto
    {
        [Required(ErrorMessage = "Status is required.")]
        public TalkingStage Status { get; set; }
    }
}
