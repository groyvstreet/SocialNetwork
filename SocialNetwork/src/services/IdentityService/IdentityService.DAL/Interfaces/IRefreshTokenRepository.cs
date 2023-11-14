using IdentityService.DAL.Entities;

namespace IdentityService.DAL.Interfaces
{
    public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
    {
        Task<RefreshToken?> GetRefreshTokenByUserIdAsync(string userId);
    }
}
