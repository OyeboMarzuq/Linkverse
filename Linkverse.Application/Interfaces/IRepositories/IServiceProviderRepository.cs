using Linkverse.Application.Common.Responses.ProviderResponses;
using Linkverse.Domain.Entities;
using Linkverse.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.Interfaces.IRepositories
{
    public interface IServiceProviderRepository
    {
        Task<ServiceProviders?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<ServiceProviders?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<(List<ServiceProviders> Items, int TotalCount)> GetPagedAsync(FilterDto filter, CancellationToken cancellationToken);
        Task<List<ServiceProviders>> GetByOccupationAsync(Occupation occupation, CancellationToken cancellationToken);
        Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task AddAsync(ServiceProviders provider, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
