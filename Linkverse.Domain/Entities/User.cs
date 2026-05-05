using Linkverse.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Domain.Entities
{
    public class User : BaseEntity
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        //public Role Role { get; set; }
        public string? RefreshToken { get; set; }
        public string Role { get; set; } = "Student";
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
