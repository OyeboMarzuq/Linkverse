using Linkverse.Application.Cache;
using Linkverse.Application.Common.Responses;
using Linkverse.Application.Common.Responses.ProviderResponses;
using Linkverse.Application.DTO.ProviderDTO;
using Linkverse.Application.Interfaces.IRepositories;
using Linkverse.Application.Interfaces.IServices;
using Linkverse.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Persistence.Services
{
    public class ServiceProviderService : IServiceProviderService
    {
        private readonly IServiceProviderRepository _repo;
        private readonly ICacheService _cache;
        private readonly ILogger<ServiceProviderService> _logger;

        private const string CachePrefix = "provider:";

        public ServiceProviderService(
            IServiceProviderRepository repo,
            ICacheService cache,
            ILogger<ServiceProviderService> logger)
        {
            _repo = repo;
            _cache = cache;
            _logger = logger;
        }

        public async Task<BaseResponse<ProviderResponseDto>> RegisterAsync(
            Guid userId, RegisterProviderDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var exists = await _repo.ExistsByUserIdAsync(userId, cancellationToken);
                if (exists)
                {
                    _logger.LogWarning("RegisterProvider — profile already exists for userId: {UserId}", userId);
                    return BaseResponse<ProviderResponseDto>.Failure(
                        "A provider profile already exists for this account.", statusCode: 409);
                }

                var provider = new ServiceProviders
                {
                    UserId = userId,
                    Type = dto.Occupation,
                    BusinessName = dto.BusinessName,
                    Description = dto.Description,
                    Location = dto.Location,
                    LicenseNumber = dto.LicenseNumber,
                    IsVerified = false,
                    BankDetails = new BankDetails
                    {
                        BankName = dto.BankDetails.BankName,
                        AccountNumber = dto.BankDetails.AccountNumber,
                        AccountName = dto.BankDetails.AccountName
                    }
                };

                await _repo.AddAsync(provider, cancellationToken);
                await _repo.SaveChangesAsync(cancellationToken);

                _cache.RemoveByPrefix(CachePrefix);

                _logger.LogInformation(
                    "Provider registered — Id: {ProviderId}, UserId: {UserId}, Occupation: {Occupation}",
                    provider.Id, userId, dto.Occupation);

                return BaseResponse<ProviderResponseDto>.Succes(
                    MapToResponse(provider), statusCode: 201);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering provider for userId: {UserId}", userId);
                return BaseResponse<ProviderResponseDto>.Failure(
                    "An error occurred while registering the provider.", statusCode: 500);
            }
        }

        public async Task<BaseResponse<ProviderResponseDto>> UpdateAsync(
            Guid userId, UpdateProviderDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var provider = await _repo.GetByUserIdAsync(userId, cancellationToken);
                if (provider is null)
                {
                    _logger.LogWarning("UpdateProvider — not found for userId: {UserId}", userId);
                    return BaseResponse<ProviderResponseDto>.Failure(
                        "Provider profile not found.", statusCode: 404);
                }

                if (dto.BusinessName is not null) provider.BusinessName = dto.BusinessName;
                if (dto.Description is not null) provider.Description = dto.Description;
                if (dto.Location is not null) provider.Location = dto.Location;
                if (dto.LicenseNumber is not null) provider.LicenseNumber = dto.LicenseNumber;

                provider.ModifiedOn = DateTime.UtcNow;

                await _repo.SaveChangesAsync(cancellationToken);

                _cache.RemoveByPrefix(CachePrefix);

                _logger.LogInformation("Provider updated — Id: {ProviderId}", provider.Id);

                return BaseResponse<ProviderResponseDto>.Succes(MapToResponse(provider));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating provider for userId: {UserId}", userId);
                return BaseResponse<ProviderResponseDto>.Failure(
                    "An error occurred while updating the provider profile.", statusCode: 500);
            }
        }

        public async Task<BaseResponse<bool>> DeleteAsync(
            Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                var provider = await _repo.GetByUserIdAsync(userId, cancellationToken);
                if (provider is null)
                {
                    _logger.LogWarning("DeleteProvider — not found for userId: {UserId}", userId);
                    return BaseResponse<bool>.Failure("Provider profile not found.", statusCode: 404);
                }

                provider.IsDeleted = true;
                provider.DeletedOn = DateTime.UtcNow;
                provider.ModifiedOn = DateTime.UtcNow;

                await _repo.SaveChangesAsync(cancellationToken);

                _cache.RemoveByPrefix(CachePrefix);

                _logger.LogInformation("Provider soft-deleted — Id: {ProviderId}", provider.Id);

                return BaseResponse<bool>.Succes(true, "Provider profile deleted.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting provider for userId: {UserId}", userId);
                return BaseResponse<bool>.Failure(
                    "An error occurred while deleting the provider profile.", statusCode: 500);
            }
        }

        public async Task<BaseResponse<ProviderResponseDto>> GetByIdAsync(
            Guid providerId, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = CacheKeys.ProviderProfile(providerId);

                var result = await _cache.GetOrSetAsync(
                    cacheKey,
                    async () =>
                    {
                        var provider = await _repo.GetByIdAsync(providerId, cancellationToken);
                        return provider is null ? null! : MapToResponse(provider);
                    },
                    CacheTTL.ProviderProfile,
                    cancellationToken);

                if (result is null)
                {
                    _logger.LogWarning("GetProvider — not found: {ProviderId}", providerId);
                    return BaseResponse<ProviderResponseDto>.Failure(
                        "Provider not found.", statusCode: 404);
                }

                return BaseResponse<ProviderResponseDto>.Succes(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching provider: {ProviderId}", providerId);
                return BaseResponse<ProviderResponseDto>.Failure(
                    "An error occurred.", statusCode: 500);
            }
        }

        public async Task<BaseResponse<ProviderResponseDto>> GetMyProfileAsync(
            Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = CacheKeys.ProviderByUser(userId);

                var result = await _cache.GetOrSetAsync(
                    cacheKey,
                    async () =>
                    {
                        var provider = await _repo.GetByUserIdAsync(userId, cancellationToken);
                        return provider is null ? null! : MapToResponse(provider);
                    },
                    CacheTTL.ProviderProfile,
                    cancellationToken);

                if (result is null)
                    return BaseResponse<ProviderResponseDto>.Failure(
                        "Provider profile not found.", statusCode: 404);

                return BaseResponse<ProviderResponseDto>.Succes(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching provider profile for userId: {UserId}", userId);
                return BaseResponse<ProviderResponseDto>.Failure(
                    "An error occurred.", statusCode: 500);
            }
        }

        public async Task<BaseResponse<PagedProviderResponseDto>> SearchAsync(
            FilterDto filter, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = $"{CachePrefix}search:{filter.ToHashKey()}";

                var result = await _cache.GetOrSetAsync(
                    cacheKey,
                    async () =>
                    {
                        var (items, total) = await _repo.GetPagedAsync(filter, cancellationToken);

                        return new PagedProviderResponseDto
                        {
                            Items = items.Select(MapToCard).ToList(),
                            TotalCount = total,
                            Page = filter.Page,
                            PageSize = filter.PageSize
                        };
                    },
                    CacheTTL.ServiceListings,
                    cancellationToken);

                return BaseResponse<PagedProviderResponseDto>.Succes(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching providers");
                return BaseResponse<PagedProviderResponseDto>.Failure(
                    "An error occurred while searching.", statusCode: 500);
            }
        }

        private static ProviderResponseDto MapToResponse(ServiceProviders p) => new()
        {
            Id = p.Id,
            UserId = p.UserId,
            BusinessName = p.BusinessName,
            Occupation = p.Type,
            Description = p.Description,
            Location = p.Location,
            LicenseNumber = p.LicenseNumber,
            IsVerified = p.IsVerified,
            Rating = p.Rating,
            ReviewCount = p.ReviewCount,
            CreatedAt = p.CreatedAt,
            BankDetails = p.BankDetails is null ? null : new BankDetailsResponseDto
            {
                Id = p.BankDetails.Id,
                BankName = p.BankDetails.BankName,
                MaskedAccountNumber = "******" + p.BankDetails.AccountNumber[^4..],
                AccountName = p.BankDetails.AccountName
            }
        };

        private static ProviderCardDto MapToCard(ServiceProviders p) => new()
        {
            Id = p.Id,
            BusinessName = p.BusinessName,
            Occupation = p.Type,
            Description = p.Description,
            Location = p.Location,
            IsVerified = p.IsVerified,
            Rating = p.Rating,
            ReviewCount = p.ReviewCount
        };
    }
}
