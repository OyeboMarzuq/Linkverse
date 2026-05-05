using Linkverse.Application.Common.Responses;
using Linkverse.Application.Common.Responses.HousingResponse;
using Linkverse.Application.DTO.HousingDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.Interfaces.IServices
{
    public interface IHousingService
    {
        Task<BaseResponse<HousingListingResponseDto>> CreateListingAsync(Guid providerId, CreateHousingListingDto dto, CancellationToken cancellationToken);
        Task<BaseResponse<HousingListingResponseDto>> UpdateListingAsync(Guid listingId, Guid requesterId, UpdateHousingListingDto dto, CancellationToken cancellationToken);
        Task<BaseResponse<HousingListingResponseDto>> GetByIdAsync(Guid listingId, CancellationToken cancellationToken);
        Task<BaseResponse<PagedHousingResponseDto>> GetAllAsync(HousingFilterDto filter, CancellationToken cancellationToken);
        Task<BaseResponse<bool>> ToggleSoldAsync(Guid listingId, Guid requesterId, CancellationToken cancellationToken);
        Task<BaseResponse<bool>> DeleteListingAsync(Guid listingId, Guid requesterId, CancellationToken cancellationToken);
    }
}
