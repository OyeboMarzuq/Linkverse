using Linkverse.Application.Common.Responses;
using Linkverse.Application.Common.Responses.UserProfileResponse;
using Linkverse.Application.DTO.UserProfileDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.Interfaces.IServices
{
    public interface IUserService
    {
        Task<BaseResponse<UserProfileResponseDto>> GetMyProfileAsync(Guid userId, CancellationToken cancellationToken);
        Task<BaseResponse<UserProfileResponseDto>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<BaseResponse<UserProfileResponseDto>> CreateProfileAsync(Guid userId, CreateUserProfileDto dto, CancellationToken cancellationToken);
        Task<BaseResponse<UserProfileResponseDto>> UpdateProfileAsync(Guid userId, UpdateUserProfileDto dto, CancellationToken cancellationToken);
        Task<BaseResponse<bool>> DeleteAccountAsync(Guid userId, CancellationToken cancellationToken);
    }
}
