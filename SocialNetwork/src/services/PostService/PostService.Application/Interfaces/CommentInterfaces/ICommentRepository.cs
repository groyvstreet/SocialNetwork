using PostService.Domain.Entities;

namespace PostService.Application.Interfaces.CommentInterfaces
{
    public interface ICommentRepository : IBaseRepository<Comment>
    {
        //Task<List<Comment>> GetCommentsAsync();

        //Task<Comment?> GetCommentByIdAsync(Guid id);

        Task<List<Comment>> GetCommentsByPostIdAsync(Guid postId);

        //Task AddCommentAsync(Comment comment);

        //Task UpdateCommentAsync(Comment comment);

        //Task RemoveCommentAsync(Comment comment);
    }
}
