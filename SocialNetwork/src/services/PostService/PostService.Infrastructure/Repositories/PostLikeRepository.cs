using Microsoft.EntityFrameworkCore;
using PostService.Application.Interfaces.PostLikeInterfaces;
using PostService.Domain.Entities;
using PostService.Infrastructure.Data;

namespace PostService.Infrastructure.Repositories
{
    public class PostLikeRepository : BaseRepository<PostLike>, IPostLikeRepository
    {
        public PostLikeRepository(DataContext context) : base(context) { }

        public async Task<List<PostLike>> GetPostLikesWithPostByUserIdAsync(Guid userId)
        {
            return await _context.PostLikes.AsNoTracking()
                .Include(postLike => postLike.Post)
                .Where(postLike => postLike.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<PostLike>> GetPostLikesWithUserByPostIdAsync(Guid postId)
        {
            return await _context.PostLikes.AsNoTracking()
                .Include(postLike => postLike.User)
                .Where(postLike => postLike.PostId == postId)
                .ToListAsync();
        }
    }
}
