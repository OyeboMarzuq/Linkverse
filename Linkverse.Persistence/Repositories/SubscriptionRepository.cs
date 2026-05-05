using Linkverse.Application.Interfaces.IRepositories;
using Linkverse.Domain.Entities;
using Linkverse.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Linkverse.Persistence.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Subscription subscription, CancellationToken cancellationToken)
        {
            await _context.Subscriptions.AddAsync(subscription, cancellationToken);
        }

        public async Task<Subscription?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Subscriptions.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<List<Subscription>> GetUserSubscriptionsAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.Subscriptions
                .Where(s => s.UserId == userId)
                .ToListAsync(cancellationToken);
        }

        public async Task<Subscription?> GetByReferenceAsync(string reference, CancellationToken cancellationToken)
        {
            return await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.PaymentReference == reference, cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Subscription subscription, CancellationToken cancellationToken)
        {
            _context.Subscriptions.Update(subscription);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<Subscription>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.Subscriptions
                .Where(s => s.UserId == userId)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Subscription>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Subscriptions.ToListAsync(cancellationToken);
        }
    }
}