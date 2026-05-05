using Linkverse.Application.Common.Responses;
using Linkverse.Application.DTO.SubscriptionDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.Interfaces.IServices
{
    public interface ISubscriptionService
    {
        Task<BaseResponse<string>> SubscribeWithWalletAsync(Guid userId, CreateSubscriptionDto dto, CancellationToken cancellationToken);
        Task<BaseResponse<string>> SubscribeWithPaystackAsync(Guid userId, CreateSubscriptionDto dto, CancellationToken cancellationToken);
        Task<BaseResponse<string>> VerifyPaystackSubscriptionAsync(string reference, CancellationToken cancellationToken);
        Task<BaseResponse<List<SubscriptionDto>>> GetMySubscriptionsAsync(Guid userId, CancellationToken cancellationToken);
    }
}
