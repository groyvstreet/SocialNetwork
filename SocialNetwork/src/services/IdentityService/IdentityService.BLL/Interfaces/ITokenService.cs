using IdentityService.BLL.DTOs.IdentityDTOs;
using System.Security.Claims;

namespace IdentityService.BLL.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);

        Task<string> GenerateRefreshTokenAsync(string userId);

        ClaimsPrincipal GetPrincipalFromToken(string token);

        Task<AuthenticatedResponseDTO> RefreshTokenAsync(string accessToken, string refreshToken);
    }
}
