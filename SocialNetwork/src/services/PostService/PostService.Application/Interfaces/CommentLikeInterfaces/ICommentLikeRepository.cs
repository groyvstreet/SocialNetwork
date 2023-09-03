using PostService.Domain.Entities;

namespace PostService.Application.Interfaces.CommentLikeInterfaces
{
    public interface ICommentLikeRepository
    {
        Task<CommentLike?> GetCommentLikeByCommentIdAndUserIdAsync(Guid commentId, Guid userId);

        Task<List<CommentLike>> GetCommentLikesByUserIdAsync(Guid userId);

        Task<List<CommentLike>> GetCommentLikesByCommentIdAsync(Guid commentId);
        
        Task<CommentLike> AddCommentLikeAsync(CommentLike commentLike);

        Task RemoveCommentLikeAsync(CommentLike commentLike);
    }
}
