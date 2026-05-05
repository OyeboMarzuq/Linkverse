using Linkverse.Application.Common.Responses;
using Linkverse.Application.Common.Responses.HousingResponse;
using Linkverse.Application.DTO.HousingDTO;
using Linkverse.Application.Interfaces;
using Linkverse.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace Linkverse.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HousingController : ControllerBase
    {
        private readonly IHousingService _housingService;

        public HousingController(IHousingService housingService)
        {
            _housingService = housingService;
        }

        // ─── Create Listing (ServiceProvider / Admin only) ────────────────────────
        [HttpPost("create")]
        [Authorize(Roles = "ServiceProvider,Admin")]
        [EnableRateLimiting("per-user")]
        [ProducesResponseType(typeof(BaseResponse<HousingListingResponseDto>), 201)]
        [ProducesResponseType(typeof(BaseResponse<HousingListingResponseDto>), 404)]
        [ProducesResponseType(typeof(BaseResponse<HousingListingResponseDto>), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> CreateListing(
            [FromBody] CreateHousingListingDto request, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty) return Unauthorized();

            var response = await _housingService.CreateListingAsync(userId, request, cancellationToken);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        // ─── Update Listing (ServiceProvider / Admin only) ────────────────────────
        [HttpPut("update/{id:guid}")]
        [Authorize(Roles = "ServiceProvider,Admin")]
        [EnableRateLimiting("per-user")]
        [ProducesResponseType(typeof(BaseResponse<HousingListingResponseDto>), 200)]
        [ProducesResponseType(typeof(BaseResponse<HousingListingResponseDto>), 404)]
        [ProducesResponseType(typeof(BaseResponse<HousingListingResponseDto>), 403)]
        [ProducesResponseType(typeof(BaseResponse<HousingListingResponseDto>), 400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> UpdateListing(
            [FromRoute] Guid id,
            [FromBody] UpdateHousingListingDto request,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty) return Unauthorized();

            var response = await _housingService.UpdateListingAsync(id, userId, request, cancellationToken);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        // ─── Get All (Everyone — filtered, searched, paginated) ───────────────────
        [HttpGet("all")]
        [AllowAnonymous]
        [EnableRateLimiting("fixed")]
        [ProducesResponseType(typeof(BaseResponse<PagedHousingResponseDto>), 200)]
        public async Task<IActionResult> GetAll(
            [FromQuery] HousingFilterDto filter, CancellationToken cancellationToken)
        {
            var response = await _housingService.GetAllAsync(filter, cancellationToken);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        // ─── Get By ID (Everyone) ─────────────────────────────────────────────────
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        [EnableRateLimiting("fixed")]
        [ProducesResponseType(typeof(BaseResponse<HousingListingResponseDto>), 200)]
        [ProducesResponseType(typeof(BaseResponse<HousingListingResponseDto>), 404)]
        public async Task<IActionResult> GetById(
            [FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var response = await _housingService.GetByIdAsync(id, cancellationToken);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        // ─── Toggle Sold / Available (ServiceProvider / Admin only) ──────────────
        [HttpPatch("toggle-sold/{id:guid}")]
        [Authorize(Roles = "ServiceProvider,Admin")]
        [EnableRateLimiting("per-user")]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        [ProducesResponseType(typeof(BaseResponse<bool>), 404)]
        [ProducesResponseType(typeof(BaseResponse<bool>), 403)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> ToggleSold(
            [FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty) return Unauthorized();

            var response = await _housingService.ToggleSoldAsync(id, userId, cancellationToken);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        // ─── Delete Listing (ServiceProvider / Admin only) ───────────────────────
        [HttpDelete("delete/{id:guid}")]
        [Authorize(Roles = "ServiceProvider,Admin")]
        [EnableRateLimiting("per-user")]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        [ProducesResponseType(typeof(BaseResponse<bool>), 404)]
        [ProducesResponseType(typeof(BaseResponse<bool>), 403)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> DeleteListing(
            [FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty) return Unauthorized();

            var response = await _housingService.DeleteListingAsync(id, userId, cancellationToken);
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