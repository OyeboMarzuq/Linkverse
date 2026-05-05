using Linkverse.Application.Cache;
using Linkverse.Application.Common.Responses;
using Linkverse.Application.Common.Responses.MatchmakingResponse;
using Linkverse.Application.DTO.MatchmakingDTO;
using Linkverse.Application.Interfaces.IRepositories;
using Linkverse.Application.Interfaces.IServices;
using Linkverse.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Persistence.Services
{
    public class MatchmakingService : IMatchmakingService
    {
        private readonly IMatchmakingRepository _repo;
        private readonly ICacheService _cache;
        private readonly ILogger<MatchmakingService> _logger;

        private const string CachePrefix = "match:";

        public MatchmakingService(
            IMatchmakingRepository repo,
            ICacheService cache,
            ILogger<MatchmakingService> logger)
        {
            _repo = repo;
            _cache = cache;
            _logger = logger;
        }

        public async Task<BaseResponse<MatchProfileResponseDto>> CreateProfileAsync(
            Guid userId, CreateMatchProfileDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var existing = await _repo.GetProfileByUserIdAsync(userId, cancellationToken);
                if (existing is not null)
                {
                    _logger.LogWarning("CreateMatchProfile — profile already exists for userId: {UserId}", userId);
                    return BaseResponse<MatchProfileResponseDto>.Failure(
                        "A match profile already exists. Use update instead.", statusCode: 409);
                }

                var profile = new MatchProfile
                {
                    UserId = userId,
                    LookingFor = dto.LookingFor,
                    ReligionPreference = dto.ReligionPreference,
                    HeightPreference = dto.HeightPreference,
                    MinAge = dto.MinAge,
                    MaxAge = dto.MaxAge,
                    Department = dto.Department,
                    Bio = dto.Bio,
                    IsActive = true
                };

                await _repo.AddProfileAsync(profile, cancellationToken);
                await _repo.SaveChangesAsync(cancellationToken);

                _cache.RemoveByPrefix(CachePrefix);

                _logger.LogInformation(
                    "Match profile created — Id: {ProfileId}, UserId: {UserId}", profile.Id, userId);

                return BaseResponse<MatchProfileResponseDto>.Succes(
                    MapToProfileResponse(profile), statusCode: 201);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating match profile for userId: {UserId}", userId);
                return BaseResponse<MatchProfileResponseDto>.Failure(
                    "An error occurred.", statusCode: 500);
            }
        }

        public async Task<BaseResponse<MatchProfileResponseDto>> UpdateProfileAsync(
            Guid userId, UpdateMatchProfileDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var profile = await _repo.GetProfileByUserIdAsync(userId, cancellationToken);
                if (profile is null)
                {
                    _logger.LogWarning("UpdateMatchProfile — not found for userId: {UserId}", userId);
                    return BaseResponse<MatchProfileResponseDto>.Failure(
                        "Match profile not found.", statusCode: 404);
                }

                if (dto.LookingFor.HasValue) profile.LookingFor = dto.LookingFor.Value;
                if (dto.ReligionPreference is not null) profile.ReligionPreference = dto.ReligionPreference;
                if (dto.HeightPreference is not null) profile.HeightPreference = dto.HeightPreference;
                if (dto.MinAge.HasValue) profile.MinAge = dto.MinAge;
                if (dto.MaxAge.HasValue) profile.MaxAge = dto.MaxAge;
                if (dto.Department is not null) profile.Department = dto.Department;
                if (dto.Bio is not null) profile.Bio = dto.Bio;
                if (dto.IsActive.HasValue) profile.IsActive = dto.IsActive.Value;

                await _repo.SaveChangesAsync(cancellationToken);

                _cache.RemoveByPrefix(CachePrefix);

                _logger.LogInformation("Match profile updated for userId: {UserId}", userId);

                return BaseResponse<MatchProfileResponseDto>.Succes(MapToProfileResponse(profile));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating match profile for userId: {UserId}", userId);
                return BaseResponse<MatchProfileResponseDto>.Failure(
                    "An error occurred.", statusCode: 500);
            }
        }

        public async Task<BaseResponse<PagedMatchResultDto>> SearchAsync(
            Guid userId, MatchSearchDto filter, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = CacheKeys.MatchResults(userId, filter.ToHashKey());

                var result = await _cache.GetOrSetAsync(
                    cacheKey,
                    async () =>
                    {
                        var (items, total) = await _repo.SearchAsync(filter, userId, cancellationToken);

                        bool isFallback = false;
                        string? fallbackMessage = null;

                        // ── Fallback — no results, relax to top 3 preferences ──────
                        if (items.Count == 0)
                        {
                            _logger.LogInformation(
                                "Match search returned 0 results for userId: {UserId}. Activating fallback.", userId);

                            items = await _repo.FallbackSearchAsync(filter, userId, cancellationToken);
                            total = items.Count;
                            isFallback = true;
                            fallbackMessage = "No exact matches found. Showing closest results based on your top preferences.";
                        }

                        var unlockedProfileIds = new HashSet<Guid>();
                        foreach (var item in items)
                        {
                            var unlocked = await _repo.GetMatchResultAsync(userId, item.Id, cancellationToken);
                            if (unlocked is not null && !unlocked.IsExpired)
                                unlockedProfileIds.Add(item.Id);
                        }

                        return new PagedMatchResultDto
                        {
                            Items = items.Select(p => BuildCard(p, unlockedProfileIds.Contains(p.Id))).ToList(),
                            TotalCount = total,
                            Page = filter.Page,
                            PageSize = filter.PageSize,
                            IsFallbackResult = isFallback,
                            FallbackMessage = fallbackMessage
                        };
                    },
                    CacheTTL.MatchResults,
                    cancellationToken);

                return BaseResponse<PagedMatchResultDto>.Succes(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching matches for userId: {UserId}", userId);
                return BaseResponse<PagedMatchResultDto>.Failure(
                    "An error occurred while searching.", statusCode: 500);
            }
        }

        public async Task<BaseResponse<UnlockTokenResponseDto>> InitiateUnlockAsync(
            Guid userId, UnlockMatchDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var profile = await _repo.GetProfileByIdAsync(dto.MatchId, cancellationToken);
                if (profile is null)
                {
                    _logger.LogWarning("InitiateUnlock — profile not found: {ProfileId}", dto.MatchId);
                    return BaseResponse<UnlockTokenResponseDto>.Failure(
                        "Match profile not found.", statusCode: 404);
                }

                var existing = await _repo.GetMatchResultAsync(userId, dto.MatchId, cancellationToken);
                if (existing is not null && !existing.IsExpired)
                {
                    _logger.LogInformation(
                        "Returning existing unlock token for userId: {UserId}, MatchId: {MatchId}",
                        userId, dto.MatchId);

                    return BaseResponse<UnlockTokenResponseDto>.Succes(new UnlockTokenResponseDto
                    {
                        UnlockToken = existing.UnlockToken,
                        ExpiresAt = existing.TokenExpiresAt,
                        Message = "You already have an active unlock for this profile."
                    });
                }

                var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48))
                    .Replace("+", "-").Replace("/", "_").Replace("=", "");

                var matchResult = new MatchResult
                {
                    SeekerId = userId,
                    UnlockedProfileId = dto.MatchId,
                    UnlockToken = token,
                    TokenExpiresAt = DateTime.UtcNow.AddHours(48)
                };

                await _repo.AddMatchResultAsync(matchResult, cancellationToken);
                await _repo.SaveChangesAsync(cancellationToken);

                _cache.RemoveByPrefix(CachePrefix);

                _logger.LogInformation(
                    "Unlock token issued — SeekerId: {UserId}, ProfileId: {ProfileId}",
                    userId, dto.MatchId);

                return BaseResponse<UnlockTokenResponseDto>.Succes(new UnlockTokenResponseDto
                {
                    UnlockToken = token,
                    ExpiresAt = matchResult.TokenExpiresAt,
                    Message = "Payment confirmed. Use the token to view the full profile."
                }, statusCode: 201);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initiating unlock for userId: {UserId}", userId);
                return BaseResponse<UnlockTokenResponseDto>.Failure(
                    "An error occurred.", statusCode: 500);
            }
        }

        public async Task<BaseResponse<UnlockedMatchProfileDto>> ViewUnlockedProfileAsync(
            ViewUnlockedMatchDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var matchResult = await _repo.GetMatchResultByTokenAsync(dto.UnlockToken, cancellationToken);

                if (matchResult is null)
                    return BaseResponse<UnlockedMatchProfileDto>.Failure(
                        "Invalid unlock token.", statusCode: 404);

                if (matchResult.IsExpired)
                {
                    _logger.LogWarning("Expired token used — SeekerId: {SeekerId}", matchResult.SeekerId);
                    return BaseResponse<UnlockedMatchProfileDto>.Failure(
                        "This unlock token has expired. Please pay again to unlock.", statusCode: 410);
                }

                var profile = matchResult.UnlockedProfile;
                var user = profile.User;

                _logger.LogInformation(
                    "Full profile viewed — ProfileId: {ProfileId}, SeekerId: {SeekerId}",
                    profile.Id, matchResult.SeekerId);

                return BaseResponse<UnlockedMatchProfileDto>.Succes(new UnlockedMatchProfileDto
                {
                    MatchProfileId = profile.Id,
                    FullName = user.UserName,
                    Religion = profile.ReligionPreference,
                    Height = profile.HeightPreference,
                    Bio = profile.Bio,
                    Location = profile.Department,
                    TokenExpiresAt = matchResult.TokenExpiresAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error viewing unlocked profile");
                return BaseResponse<UnlockedMatchProfileDto>.Failure(
                    "An error occurred.", statusCode: 500);
            }
        }

        private static MatchProfileResponseDto MapToProfileResponse(MatchProfile p) => new()
        {
            Id = p.Id,
            UserId = p.UserId,
            LookingFor = p.LookingFor,
            ReligionPreference = p.ReligionPreference,
            HeightPreference = p.HeightPreference,
            MinAge = p.MinAge,
            MaxAge = p.MaxAge,
            Department = p.Department,
            Bio = p.Bio,
            IsActive = p.IsActive,
            CreatedAt = p.CreatedAt
        };

        private static MatchResultCardDto BuildCard(MatchProfile profile, bool isUnlocked)
        {
            var user = profile.User;
            var parts = user.UserName.Split(' ');
            var firstName = parts.Length > 0 ? parts[0] : user.UserName;
            var surname = parts.Length > 1 ? parts[1] : "";

            // Mask surname: "Okeke" → "O*****"
            var maskedSurname = surname.Length > 0
                ? surname[0] + new string('*', Math.Max(surname.Length - 1, 4))
                : "*****";

            // Mask phone: "08012345678" → "080****5678"
            var phone = user.RefreshToken; // Phone stored separately — replace with UserProfile.Phone when available
            string? maskedPhone = null;
            string? fullPhone = null;

            if (!string.IsNullOrWhiteSpace(phone) && phone.Length >= 7)
            {
                maskedPhone = phone[..3] + "****" + phone[^4..];
                fullPhone = phone;
            }

            return new MatchResultCardDto
            {
                MatchId = profile.Id,
                FirstName = firstName,
                MaskedSurname = maskedSurname,
                Religion = profile.ReligionPreference,
                Height = profile.HeightPreference,
                Bio = profile.Bio,

                IsUnlocked = isUnlocked,
                Phone = isUnlocked ? fullPhone : maskedPhone,
                Location = isUnlocked ? profile.Department : "●●●●●●●●",
                PhotoUrl = isUnlocked ? null : null 
            };
        }

        /// <summary>Generates a masked phone string for display e.g. "080****5678"</summary>
        private static string MaskPhone(string phone)
            => phone.Length >= 7
                ? phone[..3] + "****" + phone[^4..]
                : "***hidden***";
    }
}
