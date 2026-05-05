using Linkverse.Application.Cache;
using Linkverse.Application.Common.Responses;
using Linkverse.Application.Common.Responses.HousingResponse;
using Linkverse.Application.DTO.HousingDTO;
using Linkverse.Application.Interfaces.IServices;
using Linkverse.Domain.Entities;
using Linkverse.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Persistence.Services
{
    public class HousingService : IHousingService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheService _cache;
        private readonly ILogger<HousingService> _logger;

        private const string CachePrefix = "housing:";

        public HousingService(
            ApplicationDbContext context,
            ICacheService cache,
            ILogger<HousingService> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        //public async Task<BaseResponse<HousingListingResponseDto>> CreateListingAsync(
        //    Guid providerId, CreateHousingListingDto dto, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        var provider = await _context.ServiceProviders
        //            .FirstOrDefaultAsync(p => p.Id == providerId, cancellationToken);

        //        if (provider is null)
        //        {
        //            _logger.LogWarning("CreateListing — provider not found: {ProviderId}", providerId);
        //            return BaseResponse<HousingListingResponseDto>.Failure("Provider not found.", statusCode: 404);
        //        }

        //        var listing = new HousingListing
        //        {
        //            ProviderId = providerId,
        //            Title = dto.Title,
        //            Type = dto.Type,
        //            Apartment = dto.Apartment,
        //            PricePerYear = dto.PricePerYear,
        //            Location = dto.Location,
        //            Description = dto.Description,
        //            Tag = dto.Tag,
        //            IsActive = true,
        //            Images = dto.Images.Select(i => new HousingImage
        //            {
        //                ImageUrl = i.ImageUrl,
        //                SortOrder = i.SortOrder
        //            }).ToList()
        //        };

        //        _context.HousingListings.Add(listing);
        //        await _context.SaveChangesAsync(cancellationToken);

        //        // Bust all housing listing caches on any write
        //        _cache.RemoveByPrefix(CachePrefix);

        //        _logger.LogInformation(
        //            "Housing listing created — Id: {ListingId}, Provider: {ProviderId}",
        //            listing.Id, providerId);

        //        return BaseResponse<HousingListingResponseDto>.Succes(
        //            await BuildResponseAsync(listing, provider.BusinessName, cancellationToken),
        //            statusCode: 201);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error creating housing listing for provider: {ProviderId}", providerId);
        //        return BaseResponse<HousingListingResponseDto>.Failure("An error occurred while creating the listing.", statusCode: 500);
        //    }
        //}

        // ─── Create Listing ───────────────────────────────────────────────────────────
        // REPLACE this method inside HousingService.cs
        public async Task<BaseResponse<HousingListingResponseDto>> CreateListingAsync(
            Guid userId, CreateHousingListingDto dto, CancellationToken cancellationToken)
        {
            try
            {
                // Resolve userId → provider record
                var provider = await _context.ServiceProviders
                    .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

                if (provider is null)
                {
                    _logger.LogWarning("CreateListing — no provider profile found for userId: {UserId}", userId);
                    return BaseResponse<HousingListingResponseDto>.Failure(
                        "Provider profile not found. Please register as a provider first.", statusCode: 404);
                }

                var listing = new HousingListing
                {
                    ProviderId = provider.Id,   // ← use provider.Id not userId
                    Title = dto.Title,
                    Type = dto.Type,
                    Apartment = dto.Apartment,
                    PricePerYear = dto.PricePerYear,
                    Location = dto.Location,
                    Description = dto.Description,
                    Tag = dto.Tag,
                    IsActive = true,
                    Images = dto.Images.Select(i => new HousingImage
                    {
                        ImageUrl = i.ImageUrl,
                        SortOrder = i.SortOrder
                    }).ToList()
                };

                _context.HousingListings.Add(listing);
                await _context.SaveChangesAsync(cancellationToken);

                _cache.RemoveByPrefix(CachePrefix);

                _logger.LogInformation(
                    "Housing listing created — Id: {ListingId}, Provider: {ProviderId}, UserId: {UserId}",
                    listing.Id, provider.Id, userId);

                return BaseResponse<HousingListingResponseDto>.Succes(
                    BuildResponse(listing, provider.BusinessName),
                    statusCode: 201);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating housing listing for userId: {UserId}", userId);
                return BaseResponse<HousingListingResponseDto>.Failure(
                    "An error occurred while creating the listing.", statusCode: 500);
            }
        }

        // Also update BuildResponseAsync to be a sync helper (no DB call needed)
        private static HousingListingResponseDto BuildResponse(HousingListing listing, string providerName) => new()
        {
            Id = listing.Id,
            ProviderId = listing.ProviderId,
            ProviderName = providerName,
            Title = listing.Title,
            Type = listing.Type,
            Apartment = listing.Apartment,
            PricePerYear = listing.PricePerYear,
            Location = listing.Location,
            Description = listing.Description,
            IsActive = listing.IsActive,
            Tag = listing.Tag,
            CreatedAt = listing.CreatedOn,
            Images = listing.Images
                .OrderBy(i => i.SortOrder)
                .Select(i => new HousingImageResponseDto
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                    SortOrder = i.SortOrder
                }).ToList()
        };

        public async Task<BaseResponse<HousingListingResponseDto>> UpdateListingAsync(
            Guid listingId, Guid requesterId, UpdateHousingListingDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var listing = await _context.HousingListings
                    .Include(h => h.Provider)
                    .Include(h => h.Images)
                    .FirstOrDefaultAsync(h => h.Id == listingId, cancellationToken);

                if (listing is null)
                {
                    _logger.LogWarning("UpdateListing — listing not found: {ListingId}", listingId);
                    return BaseResponse<HousingListingResponseDto>.Failure("Listing not found.", statusCode: 404);
                }

                // Ownership check — provider can only update their own listings
                if (listing.Provider.UserId != requesterId)
                {
                    _logger.LogWarning(
                        "UpdateListing — unauthorized. RequesterId: {RequesterId}, OwnerId: {OwnerId}",
                        requesterId, listing.Provider.UserId);
                    return BaseResponse<HousingListingResponseDto>.Failure("You are not authorised to update this listing.", statusCode: 403);
                }

                if (dto.Title is not null) listing.Title = dto.Title;
                if (dto.Type.HasValue) listing.Type = dto.Type.Value;
                if (dto.Apartment.HasValue) listing.Apartment = dto.Apartment.Value;
                if (dto.PricePerYear.HasValue) listing.PricePerYear = dto.PricePerYear.Value;
                if (dto.Location is not null) listing.Location = dto.Location;
                if (dto.Description is not null) listing.Description = dto.Description;
                if (dto.Tag is not null) listing.Tag = dto.Tag;
                if (dto.IsActive.HasValue) listing.IsActive = dto.IsActive.Value;

                listing.ModifiedOn = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);

                _cache.RemoveByPrefix(CachePrefix);

                _logger.LogInformation("Housing listing updated — Id: {ListingId}", listingId);

                return BaseResponse<HousingListingResponseDto>.Succes(
                    await BuildResponseAsync(listing, listing.Provider.BusinessName, cancellationToken));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating listing: {ListingId}", listingId);
                return BaseResponse<HousingListingResponseDto>.Failure("An error occurred while updating the listing.", statusCode: 500);
            }
        }

        public async Task<BaseResponse<HousingListingResponseDto>> GetByIdAsync(
            Guid listingId, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = CacheKeys.HousingListing(listingId);

                var result = await _cache.GetOrSetAsync(
                    cacheKey,
                    async () =>
                    {
                        var listing = await _context.HousingListings
                            .AsNoTracking()
                            .Include(h => h.Provider)
                            .Include(h => h.Images.OrderBy(i => i.SortOrder))
                            .FirstOrDefaultAsync(h => h.Id == listingId, cancellationToken);

                        if (listing is null) return null!;

                        return await BuildResponseAsync(listing, listing.Provider.BusinessName, cancellationToken);
                    },
                    CacheTTL.HousingListings,
                    cancellationToken);

                if (result is null)
                {
                    _logger.LogWarning("GetById — listing not found: {ListingId}", listingId);
                    return BaseResponse<HousingListingResponseDto>.Failure("Listing not found.", statusCode: 404);
                }

                return BaseResponse<HousingListingResponseDto>.Succes(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching listing by ID: {ListingId}", listingId);
                return BaseResponse<HousingListingResponseDto>.Failure("An error occurred while fetching the listing.", statusCode: 500);
            }
        }

        public async Task<BaseResponse<PagedHousingResponseDto>> GetAllAsync(
            HousingFilterDto filter, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = CacheKeys.HousingListings(filter.ToHashKey());

                var result = await _cache.GetOrSetAsync(
                    cacheKey,
                    async () =>
                    {
                        IQueryable<HousingListing> query = _context.HousingListings
                            .AsNoTracking()
                            .Include(h => h.Images.OrderBy(i => i.SortOrder))
                            .Where(h => h.IsActive);

                        if (filter.Type.HasValue)
                            query = query.Where(h => h.Type == filter.Type.Value);

                        if (filter.Apartment.HasValue)
                            query = query.Where(h => h.Apartment == filter.Apartment.Value);

                        if (filter.Campus.HasValue)
                        {
                            var campusName = filter.Campus.Value.ToString();
                            query = query.Where(h => h.Location.Contains(campusName));
                        }

                        if (filter.MinPrice.HasValue)
                            query = query.Where(h => h.PricePerYear >= filter.MinPrice.Value);

                        if (filter.MaxPrice.HasValue)
                            query = query.Where(h => h.PricePerYear <= filter.MaxPrice.Value);

                        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                        {
                            var term = filter.SearchTerm.ToLower();
                            query = query.Where(h =>
                                h.Title.ToLower().Contains(term) ||
                                h.Location.ToLower().Contains(term) ||
                                (h.Description != null && h.Description.ToLower().Contains(term)));
                        }

                        var totalCount = await query.CountAsync(cancellationToken);

                        var items = await query
                            .OrderByDescending(h => h.CreatedOn)
                            .Skip((filter.Page - 1) * filter.PageSize)
                            .Take(filter.PageSize)
                            .ToListAsync(cancellationToken);

                        return new PagedHousingResponseDto
                        {
                            Items = items.Select(h => new HousingListingSummaryDto
                            {
                                Id = h.Id,
                                Title = h.Title,
                                Type = h.Type,
                                Apartment = h.Apartment,
                                PricePerYear = h.PricePerYear,
                                Location = h.Location,
                                IsActive = h.IsActive,
                                Tag = h.Tag,
                                CoverImageUrl = h.Images
                                    .OrderBy(i => i.SortOrder)
                                    .FirstOrDefault()?.ImageUrl
                            }).ToList(),
                            TotalCount = totalCount,
                            Page = filter.Page,
                            PageSize = filter.PageSize
                        };
                    },
                    CacheTTL.HousingListings,
                    cancellationToken);

                return BaseResponse<PagedHousingResponseDto>.Succes(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching housing listings");
                return BaseResponse<PagedHousingResponseDto>.Failure("An error occurred while fetching listings.", statusCode: 500);
            }
        }

        public async Task<BaseResponse<bool>> ToggleSoldAsync(
            Guid listingId, Guid requesterId, CancellationToken cancellationToken)
        {
            try
            {
                var listing = await _context.HousingListings
                    .Include(h => h.Provider)
                    .FirstOrDefaultAsync(h => h.Id == listingId, cancellationToken);

                if (listing is null)
                {
                    _logger.LogWarning("ToggleSold — listing not found: {ListingId}", listingId);
                    return BaseResponse<bool>.Failure("Listing not found.", statusCode: 404);
                }

                // Only the owning provider or an admin can toggle
                if (listing.Provider.UserId != requesterId)
                {
                    _logger.LogWarning(
                        "ToggleSold — unauthorized. RequesterId: {RequesterId}, OwnerId: {OwnerId}",
                        requesterId, listing.Provider.UserId);
                    return BaseResponse<bool>.Failure("You are not authorised to perform this action.", statusCode: 403);
                }

                listing.IsActive = !listing.IsActive;
                listing.ModifiedOn = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);

                _cache.RemoveByPrefix(CachePrefix);

                var statusLabel = listing.IsActive ? "re-listed" : "marked as sold/unavailable";
                _logger.LogInformation(
                    "Listing {ListingId} {Status} by {RequesterId}", listingId, statusLabel, requesterId);

                return BaseResponse<bool>.Succes(
                    listing.IsActive,
                    $"Listing has been {statusLabel} successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling sold status for listing: {ListingId}", listingId);
                return BaseResponse<bool>.Failure("An error occurred while updating the listing status.", statusCode: 500);
            }
        }

        public async Task<BaseResponse<bool>> DeleteListingAsync(
            Guid listingId, Guid requesterId, CancellationToken cancellationToken)
        {
            try
            {
                var listing = await _context.HousingListings
                    .Include(h => h.Provider)
                    .FirstOrDefaultAsync(h => h.Id == listingId, cancellationToken);

                if (listing is null)
                {
                    _logger.LogWarning("DeleteListing — not found: {ListingId}", listingId);
                    return BaseResponse<bool>.Failure("Listing not found.", statusCode: 404);
                }

                if (listing.Provider.UserId != requesterId)
                {
                    _logger.LogWarning(
                        "DeleteListing — unauthorized. RequesterId: {RequesterId}", requesterId);
                    return BaseResponse<bool>.Failure("You are not authorised to delete this listing.", statusCode: 403);
                }

                // Soft delete
                listing.IsDeleted = true;
                listing.DeletedOn = DateTime.UtcNow;
                listing.ModifiedOn = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);

                _cache.RemoveByPrefix(CachePrefix);

                _logger.LogInformation("Listing soft-deleted — Id: {ListingId}", listingId);

                return BaseResponse<bool>.Succes(true, "Listing deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting listing: {ListingId}", listingId);
                return BaseResponse<bool>.Failure("An error occurred while deleting the listing.", statusCode: 500);
            }
        }

        private static Task<HousingListingResponseDto> BuildResponseAsync(
            HousingListing listing, string providerName, CancellationToken _)
        {
            return Task.FromResult(new HousingListingResponseDto
            {
                Id = listing.Id,
                ProviderId = listing.ProviderId,
                ProviderName = providerName,
                Title = listing.Title,
                Type = listing.Type,
                Apartment = listing.Apartment,
                PricePerYear = listing.PricePerYear,
                Location = listing.Location,
                Description = listing.Description,
                IsActive = listing.IsActive,
                Tag = listing.Tag,
                CreatedAt = listing.CreatedOn,
                Images = listing.Images
                    .OrderBy(i => i.SortOrder)
                    .Select(i => new HousingImageResponseDto
                    {
                        Id = i.Id,
                        ImageUrl = i.ImageUrl,
                        SortOrder = i.SortOrder
                    }).ToList()
            });
        }
    }
}
