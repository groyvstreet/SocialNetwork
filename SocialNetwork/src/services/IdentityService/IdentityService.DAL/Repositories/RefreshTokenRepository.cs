using IdentityService.DAL.Data;
using IdentityService.DAL.Entities;
using IdentityService.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.DAL.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly DataContext _context;

        public RefreshTokenRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> GetRefreshTokenByUserId(string userId)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == userId);
        }

        public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveRefreshTokenAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Remove(refreshToken);
            await _context.SaveChangesAsync();
        }
    }
}
