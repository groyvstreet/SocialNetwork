using IdentityService.BLL.DTOs.IdentityDTOs;
using IdentityService.BLL.Interfaces;
using IdentityService.DAL.Entities;
using IdentityService.DAL.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace IdentityService.BLL.Services
{
    public class TokenService : ITokenService
    {
        private readonly IOptions<JwtOptions> jwtOptions;
        private readonly IRefreshTokenRepository refreshTokenRepository;

        public TokenService(IOptions<JwtOptions> jwtOptions,
                            IRefreshTokenRepository refreshTokenRepository)
        {
            this.jwtOptions = jwtOptions;
            this.refreshTokenRepository = refreshTokenRepository;
        }

        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var token = new JwtSecurityToken(
                issuer: jwtOptions.Value.Issuer,
                audience: jwtOptions.Value.Audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(jwtOptions.Value.AccessTokenLifeTime)),
                signingCredentials: new SigningCredentials(jwtOptions.Value.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> GenerateRefreshTokenAsync(string userId)
        {
            var randomBytes = new byte[32];
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(randomBytes);

            var refreshToken = await refreshTokenRepository.GetRefreshTokenByUserId(userId);

            if (refreshToken is not null)
            {
                await refreshTokenRepository.RemoveRefreshTokenAsync(refreshToken);
            }

            refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                ExpirationTime = DateTime.UtcNow.Add(TimeSpan.FromMinutes(jwtOptions.Value.RefreshTokenLifeTime)),
                UserId = userId
            };

            await refreshTokenRepository.AddRefreshTokenAsync(refreshToken);

            return refreshToken.Token;
        }

        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidIssuer = jwtOptions.Value.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtOptions.Value.Audience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = jwtOptions.Value.GetSymmetricSecurityKey()
            };

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var principal = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken is null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("invalid access token");
            }

            return principal;
        }

        public async Task<AuthenticatedResponseDTO> RefreshTokenAsync(string accessToken, string refreshToken)
        {
            var principal = GetPrincipalFromToken(accessToken);
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
            {
                throw new SecurityTokenException("invalid access token");
            }

            var refreshTokenObj = await refreshTokenRepository.GetRefreshTokenByUserId(userId);

            if (refreshTokenObj is null)
            {
                throw new SecurityTokenException("invalid access token");
            }

            if (refreshToken != refreshTokenObj.Token)
            {
                throw new SecurityTokenException("invalid refresh token");
            }

            if (refreshTokenObj.ExpirationTime <= DateTime.UtcNow)
            {
                throw new SecurityTokenExpiredException("refresh token is expired");
            }

            await refreshTokenRepository.RemoveRefreshTokenAsync(refreshTokenObj);

            var authenticatedResponseDTO = new AuthenticatedResponseDTO
            {
                AccessToken = GenerateAccessToken(principal.Claims),
                RefreshToken = await GenerateRefreshTokenAsync(userId)
            };

            return authenticatedResponseDTO;
        }
    }
}
