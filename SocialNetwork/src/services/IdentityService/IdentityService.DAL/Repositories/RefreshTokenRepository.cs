using IdentityService.DAL.Data;
using IdentityService.DAL.Entities;
using IdentityService.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.DAL.Repositories
{
    public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
    {
        private readonly DataContext _context;

        public RefreshTokenRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> GetRefreshTokenByUserIdAsync(string userId)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(refreshToken => refreshToken.UserId == userId);
        }
    }
}
