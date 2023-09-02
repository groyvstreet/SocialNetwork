using PostService.Domain.Entities;

namespace PostService.Application.Interfaces.CommentsUserProfileInterfaces
{
    public interface ICommentLikeRepository
    {
        Task<CommentLike?> GetCommentLikeByIdAsync(Guid id);

        Task<CommentLike> AddCommentLikeAsync(CommentLike commentLike);

        Task RemoveCommentLikeAsync(CommentLike commentLike);
    }
}
