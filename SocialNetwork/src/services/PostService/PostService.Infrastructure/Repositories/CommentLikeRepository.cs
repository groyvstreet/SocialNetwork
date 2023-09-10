using Microsoft.EntityFrameworkCore;
using PostService.Application.Interfaces.CommentLikeInterfaces;
using PostService.Domain.Entities;
using PostService.Infrastructure.Data;

namespace PostService.Infrastructure.Repositories
{
    public class CommentLikeRepository : ICommentLikeRepository
    {
        private readonly DataContext context;

        public CommentLikeRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<CommentLike?> GetCommentLikeByCommentIdAndUserIdAsync(Guid commentId, Guid userId)
        {
            return await context.CommentLikes.AsNoTracking().FirstOrDefaultAsync(cl => cl.CommentId == commentId && cl.UserId == userId);
        }

        public async Task<List<CommentLike>> GetCommentLikesByUserIdAsync(Guid userId)
        {
            return await context.CommentLikes.AsNoTracking().Where(cl => cl.UserId == userId).ToListAsync();
        }

        public async Task<List<CommentLike>> GetCommentLikesByCommentIdAsync(Guid commentId)
        {
            return await context.CommentLikes.AsNoTracking().Where(cl => cl.CommentId == commentId).ToListAsync();
        }

        public async Task<CommentLike> AddCommentLikeAsync(CommentLike commentLike)
        {
            context.CommentLikes.Add(commentLike);
            await context.SaveChangesAsync();

            return commentLike;
        }

        public async Task RemoveCommentLikeAsync(CommentLike commentLike)
        {
            context.CommentLikes.Remove(commentLike);
            await context.SaveChangesAsync();
        }
    }
}
