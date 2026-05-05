using Linkverse.Application.Common.Responses;
using Linkverse.Application.Common.Responses.MatchmakingResponse;
using Linkverse.Application.DTO.MatchmakingDTO;
using Linkverse.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace Linkverse.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchmakingController : ControllerBase
    {
        private readonly IMatchmakingService _matchmakingService;

        public MatchmakingController(IMatchmakingService matchmakingService)
        {
            _matchmakingService = matchmakingService;
        }

        [HttpPost("profile/create")]
        [Authorize]
        [EnableRateLimiting("per-user")]
        [ProducesResponseType(typeof(BaseResponse<MatchProfileResponseDto>), 201)]
        [ProducesResponseType(typeof(BaseResponse<MatchProfileResponseDto>), 409)]
        [ProducesResponseType(typeof(BaseResponse<MatchProfileResponseDto>), 400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> CreateProfile(
            [FromBody] CreateMatchProfileDto request,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty) return Unauthorized();

            var response = await _matchmakingService.CreateProfileAsync(userId, request, cancellationToken);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [HttpPut("profile/update")]
        [Authorize]
        [EnableRateLimiting("per-user")]
        [ProducesResponseType(typeof(BaseResponse<MatchProfileResponseDto>), 200)]
        [ProducesResponseType(typeof(BaseResponse<MatchProfileResponseDto>), 404)]
        [ProducesResponseType(typeof(BaseResponse<MatchProfileResponseDto>), 400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> UpdateProfile(
            [FromBody] UpdateMatchProfileDto request,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty) return Unauthorized();

            var response = await _matchmakingService.UpdateProfileAsync(userId, request, cancellationToken);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [HttpGet("search")]
        [Authorize]
        [EnableRateLimiting("per-user")]
        [ProducesResponseType(typeof(BaseResponse<PagedMatchResultDto>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Search(
            [FromQuery] MatchSearchDto filter,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty) return Unauthorized();

            var response = await _matchmakingService.SearchAsync(userId, filter, cancellationToken);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [HttpPost("unlock/initiate")]
        [Authorize]
        [EnableRateLimiting("per-user")]
        [ProducesResponseType(typeof(BaseResponse<UnlockTokenResponseDto>), 201)]
        [ProducesResponseType(typeof(BaseResponse<UnlockTokenResponseDto>), 404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> InitiateUnlock(
            [FromBody] UnlockMatchDto request,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty) return Unauthorized();

            var response = await _matchmakingService.InitiateUnlockAsync(userId, request, cancellationToken);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [HttpPost("unlock/view")]
        [Authorize]
        [EnableRateLimiting("per-user")]
        [ProducesResponseType(typeof(BaseResponse<UnlockedMatchProfileDto>), 200)]
        [ProducesResponseType(typeof(BaseResponse<UnlockedMatchProfileDto>), 404)]
        [ProducesResponseType(typeof(BaseResponse<UnlockedMatchProfileDto>), 410)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> ViewUnlockedProfile(
            [FromBody] ViewUnlockedMatchDto request,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty) return Unauthorized();

            var response = await _matchmakingService.ViewUnlockedProfileAsync(request, cancellationToken);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        private Guid GetCurrentUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
        }
    }
}
