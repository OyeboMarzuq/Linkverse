using Linkverse.Application.Common.Responses;
using Linkverse.Application.DTO.SubscriptionDTO;
using Linkverse.Application.Interfaces.IRepositories;
using Linkverse.Application.Interfaces.IServices;
using Linkverse.Domain.Entities;
using Linkverse.Domain.Enum;

namespace Linkverse.Persistence.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IUserService _userService;
        private readonly IPaystackService _paystackService;

        public SubscriptionService(
            ISubscriptionRepository subscriptionRepository,
            IUserService userService,
            IPaystackService paystackService)
        {
            _subscriptionRepository = subscriptionRepository;
            _userService = userService;
            _paystackService = paystackService;
        }

        public async Task<BaseResponse<string>> SubscribeWithWalletAsync(
            Guid userId,
            CreateSubscriptionDto dto,
            CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserByIdAsync(userId, cancellationToken);
            if (user == null)
                return BaseResponse<string>.Failure("User not found.", statusCode: 404);

            var amount = GetPlanAmount(dto.Plan);

            var subscription = new Subscription
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Plan = dto.Plan,
                PricePerMonth = amount,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1),
                IsActive = true
            };

            await _subscriptionRepository.AddAsync(subscription, cancellationToken);
            await _subscriptionRepository.SaveChangesAsync(cancellationToken);

            return BaseResponse<string>.Succes("Subscription successful via Wallet.", statusCode: 201);
        }

        public async Task<BaseResponse<string>> SubscribeWithPaystackAsync(
            Guid userId,
            CreateSubscriptionDto dto,
            CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserByIdAsync(userId, cancellationToken);
            if (user == null)
                return BaseResponse<string>.Failure("User not found.", statusCode: 404);

            var amount = GetPlanAmount(dto.Plan);
            var reference = Guid.NewGuid().ToString();

            var callbackUrl = $"https://yourdomain.com/api/subscription/verify?reference={reference}";

            var paymentUrl = await _paystackService.InitializePaymentAsync(
                user.Email,
                amount,
                reference,
                callbackUrl);

            if (paymentUrl == null)
                return BaseResponse<string>.Failure("Failed to initialize payment.", statusCode: 500);

            var subscription = new Subscription
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Plan = dto.Plan,
                PricePerMonth = amount,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1),
                IsActive = false,
                PaymentReference = reference
            };

            await _subscriptionRepository.AddAsync(subscription, cancellationToken);
            await _subscriptionRepository.SaveChangesAsync(cancellationToken);

            return BaseResponse<string>.Succes(paymentUrl, "Payment initialized", 201);
        }

        public async Task<BaseResponse<string>> VerifyPaystackSubscriptionAsync(
            string reference,
            CancellationToken cancellationToken)
        {
            var subscription = await _subscriptionRepository.GetByReferenceAsync(reference, cancellationToken);

            if (subscription == null)
                return BaseResponse<string>.Failure("Subscription not found.", statusCode: 404);

            if (subscription.IsActive)
                return BaseResponse<string>.Succes("Already confirmed.", statusCode: 200);

            var isSuccessful = await _paystackService.VerifyPaymentAsync(reference);

            if (!isSuccessful)
                return BaseResponse<string>.Failure("Payment not successful.", statusCode: 400);

            subscription.IsActive = true;
            subscription.StartDate = DateTime.UtcNow;
            subscription.EndDate = DateTime.UtcNow.AddMonths(1);

            await _subscriptionRepository.UpdateAsync(subscription, cancellationToken);

            return BaseResponse<string>.Succes("Subscription confirmed successfully.", statusCode: 200);
        }

        public async Task<BaseResponse<List<SubscriptionDto>>> GetMySubscriptionsAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var subs = await _subscriptionRepository.GetAllByUserIdAsync(userId, cancellationToken);

            var result = subs.Select(s => new SubscriptionDto
            {
                Id = s.Id,
                UserId = s.UserId,
                Plan = s.Plan,
                PricePerMonth = s.PricePerMonth,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                IsActive = s.IsActive,
                CreatedAt = s.StartDate
            }).ToList();

            return BaseResponse<List<SubscriptionDto>>.Succes(result, "Request successful", 200);
        }

        private decimal GetPlanAmount(SubscriptionPlan plan)
        {
            return plan switch
            {
                SubscriptionPlan.Intermediate => 0,
                SubscriptionPlan.Premium => 2000,
                SubscriptionPlan.Pro => 5000,
                _ => throw new ArgumentOutOfRangeException(nameof(plan), "Unknown plan.")
            };
        }
    }
}