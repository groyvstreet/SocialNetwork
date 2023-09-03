using Microsoft.EntityFrameworkCore;
using PostService.Application.Interfaces.CommentsUserProfileInterfaces;
using PostService.Domain.Entities;

namespace PostService.Infrastructure.Data
{
    public class CommentLikeRepository : ICommentLikeRepository
    {
        private readonly DataContext context;

        public CommentLikeRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<CommentLike?> GetCommentLikeByIdAsync(Guid id)
        {
            return await context.CommentLikes.AsNoTracking().FirstOrDefaultAsync(cl => cl.Id == id);
        }

        public async Task<CommentLike?> GetCommentLikeByCommentIdAndUserProfileIdAsync(Guid commentId, Guid userProfileId)
        {
            return await context.CommentLikes.AsNoTracking().FirstOrDefaultAsync(cl => cl.CommentId == commentId && cl.UserProfileId == userProfileId);
        }

        public async Task<List<CommentLike>> GetCommentLikesByUserProfileIdAsync(Guid userProfileId)
        {
            return await context.CommentLikes.AsNoTracking().Where(cl => cl.UserProfileId == userProfileId).ToListAsync();
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
