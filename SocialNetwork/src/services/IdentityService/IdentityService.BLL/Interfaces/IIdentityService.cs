using IdentityService.BLL.DTOs.IdentityDTOs;
using IdentityService.BLL.DTOs.UserDTOs;
using System.Security.Claims;

namespace IdentityService.BLL.Interfaces
{
    public interface IIdentityService
    {
        Task<GetUserDTO> SignUpAsync(AddUserDTO addUserDTO);

        Task<AuthenticatedResponseDTO> SignInAsync(string email, string password);

        Task<AuthenticatedResponseDTO> RefreshTokenAsync(string accessToken, string refreshToken);

        string GenerateAccessToken(IEnumerable<Claim> claims);

        Task<string> GenerateRefreshTokenAsync(string userId);

        public ClaimsPrincipal GetPrincipal(string token);
    }
}
