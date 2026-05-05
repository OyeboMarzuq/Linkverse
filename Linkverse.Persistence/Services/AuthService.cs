using Linkverse.Application.Common.Responses;
using Linkverse.Application.DTO;
using Linkverse.Application.DTO.ProviderDTO;
using Linkverse.Application.DTO.UserDTO;
using Linkverse.Application.Interfaces.IServices;
using Linkverse.Domain.Entities;
using Linkverse.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Linkverse.Persistence.Services
{
    public class AuthService(ApplicationDbContext context, IConfiguration configuration) : IAuthService
    {
        // ── All three keys use "Appsettings" (lowercase s) to match appsettings.json ──
        private string JwtSecret => configuration["Appsettings:Token"]
            ?? throw new InvalidOperationException("Appsettings:Token is missing.");
        private string JwtIssuer => configuration["Appsettings:Issuer"]
            ?? throw new InvalidOperationException("Appsettings:Issuer is missing.");
        private string JwtAudience => configuration["Appsettings:Audience"]
            ?? throw new InvalidOperationException("Appsettings:Audience is missing.");

        // ─── Student Register ─────────────────────────────────────────────────────
        public async Task<User?> RegisterAsync(UserDto request)
        {
            if (await context.Users.AnyAsync(u => u.UserName == request.UserName))
                return null;

            var user = new User();
            user.UserName = request.UserName;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, request.Password);
            user.Role = "Student";

            context.Users.Add(user);
            await context.SaveChangesAsync();
            return user;
        }

        // ─── Agent / Provider Register ────────────────────────────────────────────
        public async Task<BaseResponse<TokenResponseDto>> RegisterAgentAsync(AgentRegisterDto request)
        {
            if (await context.Users.AnyAsync(u => u.UserName == request.UserName))
                return BaseResponse<TokenResponseDto>.Failure(
                    "Username already exists.", statusCode: 409);

            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var user = new User();
                user.UserName = request.UserName;
                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.PasswordHash = new PasswordHasher<User>().HashPassword(user, request.Password);
                user.Role = "ServiceProvider";

                context.Users.Add(user);
                await context.SaveChangesAsync();

                var provider = new ServiceProviders
                {
                    UserId = user.Id,
                    Type = request.Occupation,
                    BusinessName = request.BusinessName,
                    Description = request.Description,
                    Location = request.Location,
                    IsVerified = false,
                    Rating = 0,
                    ReviewCount = 0,
                    BankDetails = new BankDetails
                    {
                        BankName = request.BankName,
                        AccountNumber = request.AccountNumber,
                        AccountName = request.AccountName
                    }
                };

                context.ServiceProviders.Add(provider);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                var tokenResponse = await CreateTokenResponse(user);
                return BaseResponse<TokenResponseDto>.Succes(
                    tokenResponse, "Agent account created successfully.", statusCode: 201);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // ─── Login ────────────────────────────────────────────────────────────────
        public async Task<TokenResponseDto?> LoginAsync(LoginDto request)
        {
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.UserName == request.UserName);

            if (user is null) return null;

            if (new PasswordHasher<User>().VerifyHashedPassword(
                    user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
                return null;

            return await CreateTokenResponse(user);
        }

        // ─── Refresh Token ────────────────────────────────────────────────────────
        public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
            if (user is null) return null;

            return await CreateTokenResponse(user);
        }

        // ─── Private helpers ──────────────────────────────────────────────────────
        private async Task<TokenResponseDto> CreateTokenResponse(User user)
        {
            return new TokenResponseDto
            {
                AccessToken = CreateToken(user),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
            };
        }

        //private string CreateToken(User user)
        //{
        //    var claims = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.Name, user.UserName),
        //        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        //        new Claim(ClaimTypes.Role, user.Role ?? "Student")  // ← Role claim now included
        //    };

        //    var keyBytes = Encoding.UTF8.GetBytes(JwtSecret);
        //    var key = new SymmetricSecurityKey(keyBytes);
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        //    var tokenDescriptor = new JwtSecurityToken(
        //        issuer: JwtIssuer,
        //        audience: JwtAudience,
        //        claims: claims,
        //        expires: DateTime.UtcNow.AddDays(1),
        //        signingCredentials: creds);

        //    return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        //}

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role ?? "Student")
            };

            // "Appsettings" lowercase s — must match appsettings.json key exactly
            var secretKey = configuration["Appsettings:Token"]!;
            var issuer = configuration["Appsettings:Issuer"]!;
            var audience = configuration["Appsettings:Audience"]!;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await context.SaveChangesAsync();
            return refreshToken;
        }

        private async Task<User?> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
        {
            var user = await context.Users.FindAsync(userId);
            if (user is null || user.RefreshToken != refreshToken
                || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return null;

            return user;
        }
    }
}