using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.Common.Responses.UserProfileResponse
{
    public class UserProfileResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? Bio { get; set; }
        public string? Department { get; set; }
        public string? Faculty { get; set; }
        public int? Level { get; set; }
        public string Email { get; set; }
        public string? Location { get; set; }
    }
}
