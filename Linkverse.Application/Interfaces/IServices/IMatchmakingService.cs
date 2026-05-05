using Linkverse.Application.Common.Responses;
using Linkverse.Application.Common.Responses.MatchmakingResponse;
using Linkverse.Application.DTO.MatchmakingDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.Interfaces.IServices
{
    public interface IMatchmakingService
    {
        Task<BaseResponse<MatchProfileResponseDto>> CreateProfileAsync(Guid userId, CreateMatchProfileDto dto, CancellationToken cancellationToken);
        Task<BaseResponse<MatchProfileResponseDto>> UpdateProfileAsync(Guid userId, UpdateMatchProfileDto dto, CancellationToken cancellationToken);
        Task<BaseResponse<PagedMatchResultDto>> SearchAsync(Guid userId, MatchSearchDto filter, CancellationToken cancellationToken);
        Task<BaseResponse<UnlockTokenResponseDto>> InitiateUnlockAsync(Guid userId, UnlockMatchDto dto, CancellationToken cancellationToken);
        Task<BaseResponse<UnlockedMatchProfileDto>> ViewUnlockedProfileAsync(ViewUnlockedMatchDto dto, CancellationToken cancellationToken);
    }
}
