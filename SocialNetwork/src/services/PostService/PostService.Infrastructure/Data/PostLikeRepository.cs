using Microsoft.EntityFrameworkCore;
using PostService.Application.Interfaces.PostLikeInterfaces;
using PostService.Domain.Entities;

namespace PostService.Infrastructure.Data
{
    public class PostLikeRepository : IPostLikeRepository
    {
        private readonly DataContext context;

        public PostLikeRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<PostLike?> GetPostLikeByPostIdAndUserIdAsync(Guid postId, Guid userId)
        {
            return await context.PostLikes.AsNoTracking().FirstOrDefaultAsync(pl => pl.PostId == postId && pl.UserId == userId);
        }

        public async Task<List<PostLike>> GetPostLikesByUserIdAsync(Guid userId)
        {
            return await context.PostLikes.AsNoTracking().Where(pl => pl.UserId == userId).ToListAsync();
        }

        public async Task<List<PostLike>> GetPostLikesByPostIdAsync(Guid postId)
        {
            return await context.PostLikes.AsNoTracking().Where(pl => pl.PostId == postId).ToListAsync();
        }

        public async Task<PostLike> AddPostLikeAsync(PostLike postLike)
        {
            context.PostLikes.Add(postLike);
            await context.SaveChangesAsync();

            return postLike;
        }

        public async Task RemovePostLikeAsync(PostLike postLike)
        {
            context.PostLikes.Remove(postLike);
            await context.SaveChangesAsync();
        }
    }
}
