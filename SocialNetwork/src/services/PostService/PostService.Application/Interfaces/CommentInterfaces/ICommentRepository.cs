using PostService.Domain.Entities;

namespace PostService.Application.Interfaces.CommentInterfaces
{
    public interface ICommentRepository
    {
        Task<List<Comment>> GetCommentsAsync();

        Task<Comment?> GetCommentByIdAsync(Guid id);

        Task<List<Comment>> GetCommentsByPostIdAsync(Guid postId);

        Task<Comment> AddCommentAsync(Comment comment);

        Task<Comment> UpdateCommentAsync(Comment comment);

        Task RemoveCommentAsync(Comment comment);
    }
}
