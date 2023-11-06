using Microsoft.EntityFrameworkCore;
using PostService.Application.Interfaces.CommentLikeInterfaces;
using PostService.Domain.Entities;
using PostService.Infrastructure.Data;

namespace PostService.Infrastructure.Repositories
{
    public class CommentLikeRepository : BaseRepository<CommentLike>, ICommentLikeRepository
    {
        public CommentLikeRepository(DataContext context) : base(context) { }

        public async Task<List<CommentLike>> GetCommentLikesWithCommentByUserIdAsync(Guid userId)
        {
            return await _context.CommentLikes.AsNoTracking()
                .Include(commentLike => commentLike.Comment)
                .Where(commentLike => commentLike.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<CommentLike>> GetCommentLikesWithUserByCommentIdAsync(Guid commentId)
        {
            return await _context.CommentLikes.AsNoTracking()
                .Include(commentLike => commentLike.User)
                .Where(commentLike => commentLike.CommentId == commentId)
                .ToListAsync();
        }
    }
}
