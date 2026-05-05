using Linkverse.Application.DTO.SubscriptionDTO;
using Linkverse.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Linkverse.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpPost("wallet")]
        public async Task<IActionResult> SubscribeWithWallet(
            [FromBody] CreateSubscriptionDto dto,
            CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromToken();
            var result = await _subscriptionService.SubscribeWithWalletAsync(userId, dto, cancellationToken);
            return StatusCode(result.StatusCode ?? 200, result);
        }

        [HttpPost("paystack")]
        public async Task<IActionResult> SubscribeWithPaystack(
            [FromBody] CreateSubscriptionDto dto,
            CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromToken();
            var result = await _subscriptionService.SubscribeWithPaystackAsync(userId, dto, cancellationToken);
            return StatusCode(result.StatusCode ?? 200, result);
        }

        [HttpGet("verify")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyPaystackSubscription(
            [FromQuery] string reference,
            CancellationToken cancellationToken)
        {
            var result = await _subscriptionService.VerifyPaystackSubscriptionAsync(reference, cancellationToken);
            return StatusCode(result.StatusCode ?? 200, result);
        }

        [HttpGet("mine")]
        public async Task<IActionResult> GetMySubscriptions(CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromToken();
            var result = await _subscriptionService.GetMySubscriptionsAsync(userId, cancellationToken);
            return StatusCode(result.StatusCode ?? 200, result);
        }

        private Guid GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst("id")?.Value;
            return Guid.TryParse(userIdClaim, out var userId)
                ? userId
                : throw new UnauthorizedAccessException("Invalid token");
        }
    }
}