using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.Common.Responses.ProviderResponses
{
    public class BankDetailsResponseDto
    {
        public Guid Id { get; set; }
        public string BankName { get; set; } = string.Empty;

        /// <summary>Account number is always masked e.g. ******6789</summary>
        public string MaskedAccountNumber { get; set; } = string.Empty;

        public string AccountName { get; set; } = string.Empty;
    }
}
