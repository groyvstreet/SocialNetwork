using PostService.Domain.Entities;

namespace PostService.Application.Interfaces.PostInterfaces
{
    public interface IPostRepository : IBaseRepository<Post>
    {
        Task<Post?> GetPostWithCommentsByIdAsync(Guid id);
    }
}
