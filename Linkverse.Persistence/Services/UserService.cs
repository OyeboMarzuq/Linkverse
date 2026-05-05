using Linkverse.Application.Common.Responses;
using Linkverse.Application.Common.Responses.UserProfileResponse;
using Linkverse.Application.DTO.UserProfileDTO;
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
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheService _cache;
        private readonly ILogger<UserService> _logger;

        public UserService(
            ApplicationDbContext context,
            ICacheService cache,
            ILogger<UserService> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        public async Task<BaseResponse<UserProfileResponseDto>> GetMyProfileAsync(
            Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = $"user:profile:{userId}";

                var result = await _cache.GetOrSetAsync(
                    cacheKey,
                    async () =>
                    {
                        var profile = await _context.UserProfiles
                            .AsNoTracking()
                            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

                        if (profile is null) return null!;

                        return MapToResponse(profile);
                    },
                    TimeSpan.FromMinutes(10),
                    cancellationToken);

                if (result is null)
                {
                    _logger.LogWarning("Profile not found for userId: {UserId}", userId);
                    return BaseResponse<UserProfileResponseDto>.Failure("Profile not found.", statusCode: 404);
                }

                return BaseResponse<UserProfileResponseDto>.Succes(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching profile for userId: {UserId}", userId);
                return BaseResponse<UserProfileResponseDto>.Failure("An error occurred while fetching the profile.", statusCode: 500);
            }
        }

        public async Task<BaseResponse<UserProfileResponseDto>> GetUserByIdAsync(
            Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = $"user:profile:{userId}";

                var result = await _cache.GetOrSetAsync(
                    cacheKey,
                    async () =>
                    {
                        var profile = await _context.UserProfiles
                            .AsNoTracking()
                            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

                        return profile is null ? null! : MapToResponse(profile);
                    },
                    TimeSpan.FromMinutes(10),
                    cancellationToken);

                if (result is null)
                {
                    _logger.LogWarning("User not found — userId: {UserId}", userId);
                    return BaseResponse<UserProfileResponseDto>.Failure("User not found.", statusCode: 404);
                }

                return BaseResponse<UserProfileResponseDto>.Succes(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user by ID: {UserId}", userId);
                return BaseResponse<UserProfileResponseDto>.Failure("An error occurred.", statusCode: 500);
            }
        }

        public async Task<BaseResponse<UserProfileResponseDto>> CreateProfileAsync(
            Guid userId, CreateUserProfileDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var userExists = await _context.Users
                    .AnyAsync(u => u.Id == userId, cancellationToken);

                if (!userExists)
                {
                    _logger.LogWarning("CreateProfile — user not found: {UserId}", userId);
                    return BaseResponse<UserProfileResponseDto>.Failure("User not found.", statusCode: 404);
                }

                var existingProfile = await _context.UserProfiles
                    .AnyAsync(p => p.UserId == userId, cancellationToken);

                if (existingProfile)
                {
                    _logger.LogWarning("CreateProfile — profile already exists for userId: {UserId}", userId);
                    return BaseResponse<UserProfileResponseDto>.Failure("Profile already exists. Use update instead.", statusCode: 409);
                }

                var profile = new UserProfile
                {
                    UserId = userId,
                    Bio = dto.Bio,
                    Department = dto.Department,
                    Faculty = dto.Faculty,
                    Level = dto.Level,
                    Location = dto.Location
                };

                _context.UserProfiles.Add(profile);
                await _context.SaveChangesAsync(cancellationToken);

                _cache.Remove($"user:profile:{userId}");

                _logger.LogInformation("Profile created for userId: {UserId}", userId);

                return BaseResponse<UserProfileResponseDto>.Succes(MapToResponse(profile), statusCode: 201);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating profile for userId: {UserId}", userId);
                return BaseResponse<UserProfileResponseDto>.Failure("An error occurred while creating the profile.", statusCode: 500);
            }
        }

        public async Task<BaseResponse<UserProfileResponseDto>> UpdateProfileAsync(
            Guid userId, UpdateUserProfileDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var profile = await _context.UserProfiles
                    .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

                if (profile is null)
                {
                    _logger.LogWarning("UpdateProfile — profile not found for userId: {UserId}", userId);
                    return BaseResponse<UserProfileResponseDto>.Failure("Profile not found.", statusCode: 404);
                }

                if (dto.Bio is not null) profile.Bio = dto.Bio;
                if (dto.Department is not null) profile.Department = dto.Department;
                if (dto.Faculty is not null) profile.Faculty = dto.Faculty;
                if (dto.Level.HasValue) profile.Level = dto.Level;
                if (dto.Location is not null) profile.Location = dto.Location;

                await _context.SaveChangesAsync(cancellationToken);

                _cache.Remove($"user:profile:{userId}");

                _logger.LogInformation("Profile updated for userId: {UserId}", userId);

                return BaseResponse<UserProfileResponseDto>.Succes(MapToResponse(profile));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile for userId: {UserId}", userId);
                return BaseResponse<UserProfileResponseDto>.Failure("An error occurred while updating the profile.", statusCode: 500);
            }
        }

        public async Task<BaseResponse<bool>> DeleteAccountAsync(
            Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

                if (user is null)
                {
                    _logger.LogWarning("DeleteAccount — user not found: {UserId}", userId);
                    return BaseResponse<bool>.Failure("User not found.", statusCode: 404);
                }

                user.IsDeleted = true;
                user.DeletedOn = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);

                _cache.Remove($"user:profile:{userId}");

                _logger.LogInformation("Account soft-deleted for userId: {UserId}", userId);

                return BaseResponse<bool>.Succes(true, "Account deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting account for userId: {UserId}", userId);
                return BaseResponse<bool>.Failure("An error occurred while deleting the account.", statusCode: 500);
            }
        }

        private static UserProfileResponseDto MapToResponse(UserProfile profile) => new()
        {
            Id = profile.Id,
            UserId = profile.UserId,
            Bio = profile.Bio,
            Department = profile.Department,
            Faculty = profile.Faculty,
            Level = profile.Level,
            Location = profile.Location
        };
    }
}
