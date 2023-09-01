using PostService.Domain.Entities;

namespace PostService.Application.Interfaces.PostInterfaces
{
    public interface IPostRepository
    {
        Task<List<Post>> GetPostsAsync();

        Task<Post?> GetPostByIdAsync(Guid id);

        Task<Post> AddPostAsync(Post post);

        Task<Post> UpdatePostAsync(Post post);

        Task RemovePostAsync(Post post);
    }
}
