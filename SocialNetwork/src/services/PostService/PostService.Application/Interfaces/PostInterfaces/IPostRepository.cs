using PostService.Domain.Entities;

namespace PostService.Application.Interfaces.PostInterfaces
{
    public interface IPostRepository
    {
        Task<List<Post>> GetPostsAsync();

        Task<Post?> GetPostByIdAsync(Guid id);

        Task<List<Post>> GetPostsByUserIdAsync(Guid userProfileId);

        Task AddPostAsync(Post post);

        Task UpdatePostAsync(Post post);

        Task RemovePostAsync(Post post);
    }
}
