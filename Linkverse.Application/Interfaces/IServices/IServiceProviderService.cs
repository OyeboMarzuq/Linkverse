using Linkverse.Application.Common.Responses;
using Linkverse.Application.Common.Responses.ProviderResponses;
using Linkverse.Application.DTO.ProviderDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.Interfaces.IServices
{
    public interface IServiceProviderService
    {
        Task<BaseResponse<ProviderResponseDto>> RegisterAsync(Guid userId, RegisterProviderDto dto, CancellationToken cancellationToken);
        Task<BaseResponse<ProviderResponseDto>> UpdateAsync(Guid userId, UpdateProviderDto dto, CancellationToken cancellationToken);
        Task<BaseResponse<bool>> DeleteAsync(Guid userId, CancellationToken cancellationToken);
        Task<BaseResponse<ProviderResponseDto>> GetByIdAsync(Guid providerId, CancellationToken cancellationToken);
        Task<BaseResponse<ProviderResponseDto>> GetMyProfileAsync(Guid userId, CancellationToken cancellationToken);
        Task<BaseResponse<PagedProviderResponseDto>> SearchAsync(FilterDto filter, CancellationToken cancellationToken);
    }
}
