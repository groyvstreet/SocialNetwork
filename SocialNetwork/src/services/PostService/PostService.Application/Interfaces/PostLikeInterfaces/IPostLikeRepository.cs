using PostService.Domain.Entities;

namespace PostService.Application.Interfaces.PostLikeInterfaces
{
    public interface IPostLikeRepository
    {
        Task<PostLike?> GetPostLikeByPostIdAndUserIdAsync(Guid postId, Guid userId);

        Task<List<PostLike>> GetPostLikesByUserIdAsync(Guid userId);

        Task<List<PostLike>> GetPostLikesByPostIdAsync(Guid postId);

        Task AddPostLikeAsync(PostLike postLike);

        Task RemovePostLikeAsync(PostLike postLike);
    }
}
