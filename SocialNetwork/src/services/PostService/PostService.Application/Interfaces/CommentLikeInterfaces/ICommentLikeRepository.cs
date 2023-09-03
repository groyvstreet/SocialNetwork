using PostService.Domain.Entities;

namespace PostService.Application.Interfaces.CommentsUserProfileInterfaces
{
    public interface ICommentLikeRepository
    {
        Task<CommentLike?> GetCommentLikeByIdAsync(Guid id);

        Task<CommentLike?> GetCommentLikeByCommentIdAndUserProfileIdAsync(Guid commentId, Guid userId);

        Task<List<CommentLike>> GetCommentLikesByUserProfileIdAsync(Guid userProfileId);

        Task<List<CommentLike>> GetCommentLikesByCommentIdAsync(Guid commentId);
        
        Task<CommentLike> AddCommentLikeAsync(CommentLike commentLike);

        Task RemoveCommentLikeAsync(CommentLike commentLike);
    }
}
