using Linkverse.Application.DTO.MatchmakingDTO;
using Linkverse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.Interfaces.IRepositories
{
    public interface IMatchmakingRepository
    {
        Task<MatchProfile?> GetProfileByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<MatchProfile?> GetProfileByIdAsync(Guid profileId, CancellationToken cancellationToken);
        Task<(List<MatchProfile> Items, int TotalCount)> SearchAsync(MatchSearchDto filter, Guid excludeUserId, CancellationToken cancellationToken);
        Task<List<MatchProfile>> FallbackSearchAsync(MatchSearchDto filter, Guid excludeUserId, CancellationToken cancellationToken);
        Task<MatchResult?> GetMatchResultAsync(Guid seekerId, Guid profileId, CancellationToken cancellationToken);
        Task<MatchResult?> GetMatchResultByTokenAsync(string token, CancellationToken cancellationToken);
        Task AddProfileAsync(MatchProfile profile, CancellationToken cancellationToken);
        Task AddMatchResultAsync(MatchResult result, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
