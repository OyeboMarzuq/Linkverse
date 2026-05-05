using Linkverse.Application.Common.Responses;
using Linkverse.Application.Common.Responses.StudyPDFResponse;
using Linkverse.Application.DTO.StudyPDFDTO;
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
    public class StudyPDFController : ControllerBase
    {
        private readonly IStudyPDFService _studyPDFService;

        public StudyPDFController(IStudyPDFService studyPDFService)
        {
            _studyPDFService = studyPDFService;
        }

        [HttpPost("upload")]
        [Authorize(Roles = "Provider,Admin")]
        [EnableRateLimiting("per-user")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(BaseResponse<StudyPDFResponseDto>), 201)]
        [ProducesResponseType(typeof(BaseResponse<StudyPDFResponseDto>), 400)]
        [ProducesResponseType(typeof(BaseResponse<StudyPDFResponseDto>), 403)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Upload(
            [FromForm] UploadStudyPDFDto request,
            IFormFile file,
            CancellationToken cancellationToken)
        {
            var providerId = GetCurrentUserId();
            if (providerId == Guid.Empty) return Unauthorized();

            var response = await _studyPDFService.UploadAsync(providerId, file, request, cancellationToken);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [HttpPut("update/{id:guid}")]
        [Authorize(Roles = "Provider,Admin")]
        [EnableRateLimiting("per-user")]
        [ProducesResponseType(typeof(BaseResponse<StudyPDFResponseDto>), 200)]
        [ProducesResponseType(typeof(BaseResponse<StudyPDFResponseDto>), 404)]
        [ProducesResponseType(typeof(BaseResponse<StudyPDFResponseDto>), 403)]
        [ProducesResponseType(typeof(BaseResponse<StudyPDFResponseDto>), 400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Update(
            [FromRoute] Guid id,
            [FromBody] UpdateStudyPDFDto request,
            CancellationToken cancellationToken)
        {
            var providerId = GetCurrentUserId();
            if (providerId == Guid.Empty) return Unauthorized();

            var response = await _studyPDFService.UpdateAsync(providerId, id, request, cancellationToken);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [HttpDelete("delete/{id:guid}")]
        [Authorize(Roles = "Provider,Admin")]
        [EnableRateLimiting("per-user")]
        [ProducesResponseType(typeof(BaseResponse<bool>), 200)]
        [ProducesResponseType(typeof(BaseResponse<bool>), 404)]
        [ProducesResponseType(typeof(BaseResponse<bool>), 403)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Delete(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var providerId = GetCurrentUserId();
            if (providerId == Guid.Empty) return Unauthorized();

            var response = await _studyPDFService.DeleteAsync(providerId, id, cancellationToken);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [HttpGet("get/{id:guid}")]
        [Authorize]
        [EnableRateLimiting("fixed")]
        [ProducesResponseType(typeof(BaseResponse<StudyPDFResponseDto>), 200)]
        [ProducesResponseType(typeof(BaseResponse<StudyPDFResponseDto>), 404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty) return Unauthorized();

            var response = await _studyPDFService.GetByIdAsync(id, userId, cancellationToken);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [HttpGet("search")]
        [AllowAnonymous]
        [EnableRateLimiting("fixed")]
        [ProducesResponseType(typeof(BaseResponse<PagedStudyPDFResponseDto>), 200)]
        public async Task<IActionResult> Search(
            [FromQuery] StudyPDFFilterDto filter,
            CancellationToken cancellationToken)
        {
            var response = await _studyPDFService.SearchAsync(filter, cancellationToken);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [HttpPost("purchase")]
        [Authorize]
        [EnableRateLimiting("per-user")]
        [ProducesResponseType(typeof(BaseResponse<PDFPurchaseResponseDto>), 201)]
        [ProducesResponseType(typeof(BaseResponse<PDFPurchaseResponseDto>), 404)]
        [ProducesResponseType(typeof(BaseResponse<PDFPurchaseResponseDto>), 409)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Purchase(
            [FromBody] PurchasePDFDto request,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty) return Unauthorized();

            var response = await _studyPDFService.PurchaseAsync(userId, request, cancellationToken);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        [HttpGet("download/{id:guid}")]
        [Authorize]
        [EnableRateLimiting("per-user")]
        [ProducesResponseType(typeof(BaseResponse<string>), 200)]
        [ProducesResponseType(typeof(BaseResponse<string>), 403)]
        [ProducesResponseType(typeof(BaseResponse<string>), 404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetDownloadUrl(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty) return Unauthorized();

            var response = await _studyPDFService.GetDownloadUrlAsync(userId, id, cancellationToken);
            return StatusCode(response.StatusCode ?? 200, response);
        }

        private Guid GetCurrentUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
        }
    }
}
