using Linkverse.Application.DTO;
using Linkverse.Application.DTO.UserDTO;
using Linkverse.Application.Interfaces;
using Linkverse.Domain.Entities;
using Linkverse.Persistence.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {

        [HttpPost("register")]
        public async Task<ActionResult<User>> Regsister(UserDto request)
        {
            var user = await authService.RegisterAsync(request);
            if (user is null)
                return BadRequest("Username Already Exists.");

            return Ok(user);

        }

        [HttpPost("login")]
        [EnableRateLimiting("per-user")]
        public async Task<ActionResult<TokenResponseDto>> Login(UserDto request)
        {
            var result = await authService.LoginAsync(request);
            if (result is null)
                return BadRequest("Invalid Username Or Password.");

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await authService.RefreshTokenAsync(request);
            if(result is null || result is null || result.RefreshToken is null)
                return Unauthorized("Invalid refresh token");

            return Ok(result);
        }

        //private string CreateToken(User user)
        //{
        //    var claims = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.Name, user.UserName)
        //    };

        //    var key = new SymmetricSecurityKey(
        //        Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        //    var tokenDescriptor = new JwtSecurityToken(
        //        issuer: configuration.GetValue<string>("AppSettings:Issuer"),
        //        audience: configuration.GetValue<string>("AppSettings:Audience"),
        //        claims: claims,
        //        expires: DateTime.UtcNow.AddDays(1),
        //        signingCredentials: creds
        //        );
        //    return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        //}

        [Authorize]
        [HttpGet]
        public IActionResult AuthenticatedOnlyEndPoint()
        {
            return Ok("You're Authenticated!");
        }

        [Authorize("Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnlyEndpoint()
        {
            return Ok("You're An Admin!");
        }
    }
}
