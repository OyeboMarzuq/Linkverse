using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.Interfaces.IServices
{
    public interface IPaystackService
    {
        Task<string?> InitializePaymentAsync(string email, decimal amount, string reference, string callbackUrl);
        Task<bool> VerifyPaymentAsync(string reference);
    }
}
