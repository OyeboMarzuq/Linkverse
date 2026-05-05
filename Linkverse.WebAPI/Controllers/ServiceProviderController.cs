using Linkverse.Application.Common.Responses;
using Linkverse.Application.Common.Responses.ProviderResponses;
using Linkverse.Application.DTO.ProviderDTO;
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
    public class ServiceProviderController : ControllerBase
    {
        private readonly IServiceProviderService _providerService;

        public ServiceProviderController(IServiceProviderService providerService)
        {
            _providerService = providerService;
        }

        [HttpPost("register")]
        [Authorize]
        [EnableRateLimiting("per-user")]
        [ProducesResponseType(typeof(BaseResponse<ProviderResponseDto>), 201)]
        [ProducesResponseType(typeof(BaseResponse<ProviderResponseDto>), 409)]
        [ProducesResponseType(typeof(BaseResponse<ProviderResponseDto>), 400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Register(
            [FromBody] RegisterProviderDto request, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty) return Unauthorized();

            var response = await _providerService.RegisterAsync(userId, request, cancellationToken);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [HttpPut("update")]
        [Authorize]
        [EnableRateLimiting("per-user")]
        [ProducesResponseType(typeof(BaseResponse<ProviderResponseDto>), 200)]
        [ProducesResponseType(typeof(BaseResponse<ProviderResponseDto>), 404)]
        [ProducesResponseType(typeof(BaseResponse<ProviderResponseDto>), 400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Update(
            [FromBody] UpdateProviderDto request, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty) return Unauthorized();

            var response = await _providerService.UpdateAsync(userId, request, cancellationToken);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [HttpDelete("delete")]
        [Authorize]
        [EnableRateLimiting("per-user")]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        [ProducesResponseType(typeof(BaseResponse<bool>), 404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty) return Unauthorized();

            var response = await _providerService.DeleteAsync(userId, cancellationToken);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [HttpGet("me")]
        [Authorize]
        [EnableRateLimiting("per-user")]
        [ProducesResponseType(typeof(BaseResponse<ProviderResponseDto>), 200)]
        [ProducesResponseType(typeof(BaseResponse<ProviderResponseDto>), 404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty) return Unauthorized();

            var response = await _providerService.GetMyProfileAsync(userId, cancellationToken);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [HttpGet("get/{id:guid}")]
        [AllowAnonymous]
        [EnableRateLimiting("fixed")]
        [ProducesResponseType(typeof(BaseResponse<ProviderResponseDto>), 200)]
        [ProducesResponseType(typeof(BaseResponse<ProviderResponseDto>), 404)]
        public async Task<IActionResult> GetById(
            [FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var response = await _providerService.GetByIdAsync(id, cancellationToken);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [HttpGet("search")]
        [AllowAnonymous]
        [EnableRateLimiting("fixed")]
        [ProducesResponseType(typeof(BaseResponse<PagedProviderResponseDto>), 200)]
        public async Task<IActionResult> Search(
            [FromQuery] FilterDto filter, CancellationToken cancellationToken)
        {
            var response = await _providerService.SearchAsync(filter, cancellationToken);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        private Guid GetCurrentUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
        }
    }
}
