using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Domain.Entities
{
    public class UserProfile
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string? Bio { get; set; }
        public string? Department { get; set; }
        public string? Faculty { get; set; }
        public int? Level { get; set; }
        public string? Location { get; set; }

        public User User { get; set; } = null!;
    }
}
