using Linkverse.Application.Common.Responses;
using Linkverse.Application.DTO;
using Linkverse.Application.DTO.ProviderDTO;
using Linkverse.Application.DTO.UserDTO;
using Linkverse.Domain.Entities;
using System.Security.Claims;

namespace Linkverse.Application.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserDto request);
        Task<BaseResponse<TokenResponseDto>> RegisterAgentAsync(AgentRegisterDto request);
        Task<TokenResponseDto?> LoginAsync(LoginDto request);
        //Task<BaseResponse<string>> GoogleLoginAsync(IEnumerable<Claim> claims);
        Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request);
    }
}
