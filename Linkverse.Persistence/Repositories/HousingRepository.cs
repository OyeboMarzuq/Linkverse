using Linkverse.Application.DTO.HousingDTO;
using Linkverse.Application.Interfaces.IRepositories;
using Linkverse.Domain.Entities;
using Linkverse.Persistence.Context;
using Microsoft.EntityFrameworkCore;


namespace Linkverse.Persistence.Repositories
{
    public class HousingRepository : IHousingRepository
    {
        private readonly ApplicationDbContext _context;

        public HousingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<HousingListing?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
            => await _context.HousingListings
                .AsNoTracking()
                .Include(h => h.Provider)
                .Include(h => h.Images.OrderBy(i => i.SortOrder))
                .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);

        public async Task<(List<HousingListing> Items, int TotalCount)> GetPagedAsync(
            HousingFilterDto filter, CancellationToken cancellationToken)
        {
            var query = _context.HousingListings
                .AsNoTracking()
                .Include(h => h.Images.OrderBy(i => i.SortOrder))
                .Include(h => h.Provider)
                .Where(h => h.IsActive)
                .AsQueryable();

            if (filter.Type.HasValue)
                query = query.Where(h => h.Type == filter.Type.Value);

            if (filter.Apartment.HasValue)
                query = query.Where(h => h.Apartment == filter.Apartment.Value);

            if (filter.MinPrice.HasValue)
                query = query.Where(h => h.PricePerYear >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(h => h.PricePerYear <= filter.MaxPrice.Value);

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.Trim().ToLower();
                query = query.Where(h =>
                    h.Title.ToLower().Contains(term) ||
                    h.Location.ToLower().Contains(term) ||
                    (h.Description != null && h.Description.ToLower().Contains(term)));
            }

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(h => h.CreatedOn)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync(cancellationToken);

            return (items, total);
        }

        public async Task<List<HousingListing>> GetByProviderAsync(Guid providerId, CancellationToken cancellationToken)
            => await _context.HousingListings
                .AsNoTracking()
                .Include(h => h.Images.OrderBy(i => i.SortOrder))
                .Where(h => h.ProviderId == providerId)
                .OrderByDescending(h => h.CreatedOn)
                .ToListAsync(cancellationToken);

        public async Task AddAsync(HousingListing listing, CancellationToken cancellationToken)
            => await _context.HousingListings.AddAsync(listing, cancellationToken);

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
            => await _context.SaveChangesAsync(cancellationToken);
    }
}
