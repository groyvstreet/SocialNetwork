using PostService.Domain.Entities;

namespace PostService.Application.Interfaces.PostLikeInterfaces
{
    public interface IPostLikeRepository : IBaseRepository<PostLike>
    {
        Task<List<PostLike>> GetPostLikesWithPostByUserIdAsync(Guid userId);

        Task<List<PostLike>> GetPostLikesWithUserByPostIdAsync(Guid postId);
    }
}
