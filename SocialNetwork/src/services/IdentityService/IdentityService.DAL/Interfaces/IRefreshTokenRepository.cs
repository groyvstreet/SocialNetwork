using IdentityService.DAL.Entities;

namespace IdentityService.DAL.Interfaces
{
    public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
    {
        Task<RefreshToken?> GetRefreshTokenByUserId(string userId);
    }
}
