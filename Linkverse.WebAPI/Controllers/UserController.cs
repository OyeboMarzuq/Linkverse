using Linkverse.Application.Common.Responses;
using Linkverse.Application.Common.Responses.UserProfileResponse;
using Linkverse.Application.DTO.UserProfileDTO;
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
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("me")]
        [EnableRateLimiting("per-user")]
        [ProducesResponseType(typeof(BaseResponse<UserProfileResponseDto>), 200)]
        [ProducesResponseType(typeof(BaseResponse<UserProfileResponseDto>), 404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return Unauthorized();

            var response = await _userService.GetMyProfileAsync(userId, cancellationToken);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(BaseResponse<UserProfileResponseDto>), 200)]
        [ProducesResponseType(typeof(BaseResponse<UserProfileResponseDto>), 404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetUserById([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var response = await _userService.GetUserByIdAsync(id, cancellationToken);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        // ─── Create Profile ───────────────────────────────────────────────────────
        [HttpPost("profile")]
        [EnableRateLimiting("per-user")]
        [ProducesResponseType(typeof(BaseResponse<UserProfileResponseDto>), 201)]
        [ProducesResponseType(typeof(BaseResponse<UserProfileResponseDto>), 409)]
        [ProducesResponseType(typeof(BaseResponse<UserProfileResponseDto>), 400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> CreateProfile(
            [FromBody] CreateUserProfileDto request, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return Unauthorized();

            var response = await _userService.CreateProfileAsync(userId, request, cancellationToken);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        // ─── Update Profile ───────────────────────────────────────────────────────
        [HttpPut("profile")]
        [EnableRateLimiting("per-user")]
        [ProducesResponseType(typeof(BaseResponse<UserProfileResponseDto>), 200)]
        [ProducesResponseType(typeof(BaseResponse<UserProfileResponseDto>), 404)]
        [ProducesResponseType(typeof(BaseResponse<UserProfileResponseDto>), 400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> UpdateProfile(
            [FromBody] UpdateUserProfileDto request, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return Unauthorized();

            var response = await _userService.UpdateProfileAsync(userId, request, cancellationToken);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        // ─── Delete Account ───────────────────────────────────────────────────────
        [HttpDelete("account")]
        [EnableRateLimiting("per-user")]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        [ProducesResponseType(typeof(BaseResponse<bool>), 404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> DeleteAccount(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return Unauthorized();

            var response = await _userService.DeleteAccountAsync(userId, cancellationToken);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        // ─── Helper ───────────────────────────────────────────────────────────────
        private Guid GetCurrentUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
        }
    }
}
