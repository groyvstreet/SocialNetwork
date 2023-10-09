using PostService.Application.DTOs.PostDTOs;

namespace PostService.Application.Interfaces.PostInterfaces
{
    public interface IPostService
    {
        Task<List<GetPostDTO>> GetPostsAsync();

        Task<GetPostDTO> GetPostByIdAsync(Guid id);

        Task<List<GetPostDTO>> GetPostsByUserIdAsync(Guid userId);

        Task<List<GetPostDTO>> GetLikedPostsByUserIdAsync(Guid userId);

        Task<GetPostDTO> AddPostAsync(AddPostDTO addPostDTO, Guid authenticatedUserId);

        Task<GetPostDTO> UpdatePostAsync(UpdatePostDTO updatePostDTO, Guid authenticatedUserId);

        Task RemovePostByIdAsync(Guid id, Guid authenticatedUserId);
    }
}
