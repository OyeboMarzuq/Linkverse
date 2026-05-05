using Linkverse.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.Common.Responses.ProviderResponses
{
    public class ProviderResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public Occupation? Occupation { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? LicenseNumber { get; set; }
        public bool IsVerified { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public BankDetailsResponseDto? BankDetails { get; set; }
    }
}
