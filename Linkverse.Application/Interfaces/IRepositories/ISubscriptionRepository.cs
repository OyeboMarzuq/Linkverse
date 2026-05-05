using Linkverse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.Interfaces.IRepositories
{
    public interface ISubscriptionRepository
    {
        Task AddAsync(Subscription subscription, CancellationToken cancellationToken);
        Task UpdateAsync(Subscription subscription, CancellationToken cancellationToken);

        Task<Subscription?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<List<Subscription>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<List<Subscription>> GetAllAsync(CancellationToken cancellationToken);
        Task<List<Subscription>> GetUserSubscriptionsAsync(Guid userId, CancellationToken cancellationToken);
        Task<Subscription?> GetByReferenceAsync(string reference, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);

    }
}
