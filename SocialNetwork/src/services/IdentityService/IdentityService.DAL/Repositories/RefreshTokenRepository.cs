using IdentityService.DAL.Entities;
using IdentityService.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.DAL.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly DataContext context;

        public RefreshTokenRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<RefreshToken?> GetRefreshTokenByUserId(string userId)
        {
            return await context.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == userId);
        }

        public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
        {
            context.RefreshTokens.Add(refreshToken);
            await context.SaveChangesAsync();
        }

        public async Task RemoveRefreshTokenAsync(RefreshToken refreshToken)
        {
            context.RefreshTokens.Remove(refreshToken);
            await context.SaveChangesAsync();
        }
    }
}
