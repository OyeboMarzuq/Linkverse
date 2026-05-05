using Linkverse.Application.DTO.MatchmakingDTO;
using Linkverse.Application.Interfaces.IRepositories;
using Linkverse.Domain.Entities;
using Linkverse.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Persistence.Repositories
{
    public class MatchmakingRepository : IMatchmakingRepository
    {
        private readonly ApplicationDbContext _context;

        public MatchmakingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MatchProfile?> GetProfileByUserIdAsync(Guid userId, CancellationToken cancellationToken)
            => await _context.MatchProfiles
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.UserId == userId, cancellationToken);

        public async Task<MatchProfile?> GetProfileByIdAsync(Guid profileId, CancellationToken cancellationToken)
            => await _context.MatchProfiles
                .AsNoTracking()
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == profileId, cancellationToken);

        public async Task<(List<MatchProfile> Items, int TotalCount)> SearchAsync(
            MatchSearchDto filter, Guid excludeUserId, CancellationToken cancellationToken)
        {
            var query = BuildBaseQuery(filter, excludeUserId);

            var total = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(m => m.CreatedAt)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync(cancellationToken);

            return (items, total);
        }

        public async Task<List<MatchProfile>> FallbackSearchAsync(
            MatchSearchDto filter, Guid excludeUserId, CancellationToken cancellationToken)
        {
            var query = _context.MatchProfiles
                .AsNoTracking()
                .Include(m => m.User)
                .Where(m => m.IsActive && m.UserId != excludeUserId)
                .Where(m => m.LookingFor == filter.LookingFor);

            // Apply only the top 3 preferences — OR logic (match any of the three)
            query = query.Where(m =>
                (filter.ReligionPreference != null &&
                 m.ReligionPreference != null &&
                 m.ReligionPreference.ToLower() == filter.ReligionPreference.ToLower())
                ||
                (filter.HeightPreference != null &&
                 m.HeightPreference != null &&
                 m.HeightPreference == filter.HeightPreference)
                ||
                (filter.Department != null &&
                 m.Department != null &&
                 m.Department.ToLower().Contains(filter.Department.ToLower())));

            return await query
                .OrderByDescending(m => m.CreatedAt)
                .Take(10)
                .ToListAsync(cancellationToken);
        }

        public async Task<MatchResult?> GetMatchResultAsync(
            Guid seekerId, Guid profileId, CancellationToken cancellationToken)
            => await _context.MatchResults
                .FirstOrDefaultAsync(r =>
                    r.SeekerId == seekerId &&
                    r.UnlockedProfileId == profileId,
                    cancellationToken);

        public async Task<MatchResult?> GetMatchResultByTokenAsync(
            string token, CancellationToken cancellationToken)
            => await _context.MatchResults
                .Include(r => r.UnlockedProfile)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(r => r.UnlockToken == token, cancellationToken);

        public async Task AddProfileAsync(MatchProfile profile, CancellationToken cancellationToken)
            => await _context.MatchProfiles.AddAsync(profile, cancellationToken);

        public async Task AddMatchResultAsync(MatchResult result, CancellationToken cancellationToken)
            => await _context.MatchResults.AddAsync(result, cancellationToken);

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
            => await _context.SaveChangesAsync(cancellationToken);

        // ─── Private helpers ──────────────────────────────────────────────────────
        private IQueryable<MatchProfile> BuildBaseQuery(MatchSearchDto filter, Guid excludeUserId)
        {
            var query = _context.MatchProfiles
                .AsNoTracking()
                .Include(m => m.User)
                .Where(m => m.IsActive && m.UserId != excludeUserId)
                .Where(m => m.LookingFor == filter.LookingFor)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.ReligionPreference))
                query = query.Where(m =>
                    m.ReligionPreference != null &&
                    m.ReligionPreference.ToLower() == filter.ReligionPreference.ToLower());

            if (!string.IsNullOrWhiteSpace(filter.HeightPreference))
                query = query.Where(m =>
                    m.HeightPreference != null &&
                    m.HeightPreference == filter.HeightPreference);

            if (!string.IsNullOrWhiteSpace(filter.Department))
                query = query.Where(m =>
                    m.Department != null &&
                    m.Department.ToLower().Contains(filter.Department.ToLower()));

            if (filter.MinAge.HasValue)
                query = query.Where(m => m.MinAge >= filter.MinAge.Value);

            if (filter.MaxAge.HasValue)
                query = query.Where(m => m.MaxAge <= filter.MaxAge.Value);

            return query;
        }
    }
}
