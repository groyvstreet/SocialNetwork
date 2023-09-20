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
        private readonly IOptions<JwtOptions> _jwtOptions;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public TokenService(IOptions<JwtOptions> jwtOptions,
                            IRefreshTokenRepository refreshTokenRepository)
        {
            _jwtOptions = jwtOptions;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Value.Issuer,
                audience: _jwtOptions.Value.Audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(_jwtOptions.Value.AccessTokenLifeTime)),
                signingCredentials: new SigningCredentials(_jwtOptions.Value.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> GenerateRefreshTokenAsync(string userId)
        {
            var randomBytes = new byte[32];
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(randomBytes);

            var refreshToken = await _refreshTokenRepository.GetRefreshTokenByUserId(userId);

            if (refreshToken is not null)
            {
                _refreshTokenRepository.Remove(refreshToken);
            }

            refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                ExpirationTime = DateTime.UtcNow.Add(TimeSpan.FromMinutes(_jwtOptions.Value.RefreshTokenLifeTime)),
                UserId = userId
            };

            await _refreshTokenRepository.AddAsync(refreshToken);
            await _refreshTokenRepository.SaveChangesAsync();

            return refreshToken.Token;
        }

        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidIssuer = _jwtOptions.Value.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtOptions.Value.Audience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _jwtOptions.Value.GetSymmetricSecurityKey()
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

            var refreshTokenObj = await _refreshTokenRepository.GetRefreshTokenByUserId(userId);

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

            _refreshTokenRepository.Remove(refreshTokenObj);
            await _refreshTokenRepository.SaveChangesAsync();

            var authenticatedResponseDTO = new AuthenticatedResponseDTO
            {
                AccessToken = GenerateAccessToken(principal.Claims),
                RefreshToken = await GenerateRefreshTokenAsync(userId)
            };

            return authenticatedResponseDTO;
        }
    }
}
