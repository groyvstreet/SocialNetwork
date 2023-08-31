using PostService.Application.DTOs.PostDTOs;

namespace PostService.Application.Interfaces
{
    public interface IPostService
    {
        Task<List<GetPostDTO>> GetPostsAsync();

        Task<GetPostDTO?> GetPostByIdAsync(Guid id);

        Task AddPostAsync(AddPostDTO addPostDTO);

        Task<bool> UpdatePostAsync(UpdatePostDTO updatePostDTO);

        Task<bool> RemovePostByIdAsync(Guid id);
    }
}
