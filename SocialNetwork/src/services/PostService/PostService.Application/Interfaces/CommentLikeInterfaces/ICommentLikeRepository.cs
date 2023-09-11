using PostService.Domain.Entities;

namespace PostService.Application.Interfaces.CommentLikeInterfaces
{
    public interface ICommentLikeRepository : IBaseRepository<CommentLike>
    {
        Task<CommentLike?> GetCommentLikeByCommentIdAndUserIdAsync(Guid commentId, Guid userId);

        Task<List<CommentLike>> GetCommentLikesByUserIdAsync(Guid userId);

        Task<List<CommentLike>> GetCommentLikesByCommentIdAsync(Guid commentId);
        
        //Task AddCommentLikeAsync(CommentLike commentLike);

        //Task RemoveCommentLikeAsync(CommentLike commentLike);
    }
}
