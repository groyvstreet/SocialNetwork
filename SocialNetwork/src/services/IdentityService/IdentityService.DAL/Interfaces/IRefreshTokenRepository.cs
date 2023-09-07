using IdentityService.DAL.Entities;

namespace IdentityService.DAL.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetRefreshTokenByUserId(string userId);

        Task AddRefreshTokenAsync(RefreshToken refreshToken);

        Task RemoveRefreshTokenAsync(RefreshToken refreshToken);
    }
}
