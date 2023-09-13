using PostService.Domain.Entities;

namespace PostService.Application.Interfaces.CommentLikeInterfaces
{
    public interface ICommentLikeRepository : IBaseRepository<CommentLike>
    {
        Task<List<CommentLike>> GetCommentLikesWithCommentByUserIdAsync(Guid userId);

        Task<List<CommentLike>> GetCommentLikesWithUserByCommentIdAsync(Guid commentId);
    }
}
