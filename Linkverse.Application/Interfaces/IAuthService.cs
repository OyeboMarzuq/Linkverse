using Linkverse.Application.DTO;
using Linkverse.Application.DTO.UserDTO;
using Linkverse.Domain.Entities;

namespace Linkverse.Persistence.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserDto request);
        Task<TokenResponseDto?> LoginAsync(UserDto request);
        Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request);
    }
}
