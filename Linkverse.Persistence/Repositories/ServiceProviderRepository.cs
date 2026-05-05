using Linkverse.Application.Common.Responses.ProviderResponses;
using Linkverse.Application.Interfaces.IRepositories;
using Linkverse.Domain.Entities;
using Linkverse.Domain.Enum;
using Linkverse.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Persistence.Repositories
{
    public class ServiceProviderRepository : IServiceProviderRepository
    {
        private readonly ApplicationDbContext _context;

        public ServiceProviderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceProviders?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
            => await _context.ServiceProviders
                .AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.BankDetails)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        public async Task<ServiceProviders?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
            => await _context.ServiceProviders
                .Include(p => p.User)
                .Include(p => p.BankDetails)
                .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

        public async Task<(List<ServiceProviders> Items, int TotalCount)> GetPagedAsync(
            FilterDto filter, CancellationToken cancellationToken)
        {
            var query = _context.ServiceProviders
                .AsNoTracking()
                .Include(p => p.User)
                .AsQueryable();

            if (filter.Occupation.HasValue)
                query = query.Where(p => p.Type == filter.Occupation.Value);

            if (filter.VerifiedOnly)
                query = query.Where(p => p.IsVerified);

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.Trim().ToLower();
                query = query.Where(p =>
                    p.BusinessName.ToLower().Contains(term) ||
                    (p.Description != null && p.Description.ToLower().Contains(term)) ||
                    (p.Location != null && p.Location.ToLower().Contains(term)));
            }

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(p => p.Rating)
                .ThenByDescending(p => p.ReviewCount)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync(cancellationToken);

            return (items, total);
        }

        public async Task<List<ServiceProviders>> GetByOccupationAsync(
            Occupation occupation, CancellationToken cancellationToken)
            => await _context.ServiceProviders
                .AsNoTracking()
                .Include(p => p.User)
                .Where(p => p.Type == occupation)
                .OrderByDescending(p => p.Rating)
                .ToListAsync(cancellationToken);

        public async Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken)
            => await _context.ServiceProviders
                .AnyAsync(p => p.UserId == userId, cancellationToken);

        public async Task AddAsync(ServiceProviders provider, CancellationToken cancellationToken)
            => await _context.ServiceProviders.AddAsync(provider, cancellationToken);

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
            => await _context.SaveChangesAsync(cancellationToken);
    }
}
