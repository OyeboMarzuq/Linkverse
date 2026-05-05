using Linkverse.Application.DTO.HousingDTO;
using Linkverse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.Interfaces.IRepositories
{
    public interface IHousingRepository
    {
        Task<HousingListing?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<(List<HousingListing> Items, int TotalCount)> GetPagedAsync(HousingFilterDto filter, CancellationToken cancellationToken);
        Task<List<HousingListing>> GetByProviderAsync(Guid providerId, CancellationToken cancellationToken);
        Task AddAsync(HousingListing listing, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
