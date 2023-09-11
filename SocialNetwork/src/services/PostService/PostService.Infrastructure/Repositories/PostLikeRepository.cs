using Microsoft.EntityFrameworkCore;
using PostService.Application.Interfaces.PostLikeInterfaces;
using PostService.Domain.Entities;
using PostService.Infrastructure.Data;

namespace PostService.Infrastructure.Repositories
{
    public class PostLikeRepository : BaseRepository<PostLike>, IPostLikeRepository
    {
        private readonly DataContext _context;

        public PostLikeRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PostLike?> GetPostLikeByPostIdAndUserIdAsync(Guid postId, Guid userId)
        {
            return await _context.PostLikes.AsNoTracking().FirstOrDefaultAsync(pl => pl.PostId == postId && pl.UserId == userId);
        }

        public async Task<List<PostLike>> GetPostLikesByUserIdAsync(Guid userId)
        {
            return await _context.PostLikes.AsNoTracking().Where(pl => pl.UserId == userId).ToListAsync();
        }

        public async Task<List<PostLike>> GetPostLikesByPostIdAsync(Guid postId)
        {
            return await _context.PostLikes.AsNoTracking().Where(pl => pl.PostId == postId).ToListAsync();
        }

        /*public async Task AddPostLikeAsync(PostLike postLike)
        {
            _context.PostLikes.Add(postLike);
            await _context.SaveChangesAsync();
        }*/

        /*public async Task RemovePostLikeAsync(PostLike postLike)
        {
            _context.PostLikes.Remove(postLike);
            await _context.SaveChangesAsync();
        }*/
    }
}
