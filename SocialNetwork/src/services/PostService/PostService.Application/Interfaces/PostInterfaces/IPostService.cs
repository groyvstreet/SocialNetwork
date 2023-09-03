using PostService.Application.DTOs.PostDTOs;

namespace PostService.Application.Interfaces.PostInterfaces
{
    public interface IPostService
    {
        Task<List<GetPostDTO>> GetPostsAsync();

        Task<GetPostDTO> GetPostByIdAsync(Guid id);

        Task<List<GetPostDTO>> GetPostsByUserIdAsync(Guid userId);

        Task<List<GetPostDTO>> GetLikedPostsByUserIdAsync(Guid userId);

        Task<GetPostDTO> AddPostAsync(AddPostDTO addPostDTO);

        Task<GetPostDTO> UpdatePostAsync(UpdatePostDTO updatePostDTO);

        Task RemovePostByIdAsync(Guid id);
    }
}
