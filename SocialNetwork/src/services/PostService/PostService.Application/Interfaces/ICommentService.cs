using PostService.Application.DTOs.CommentDTOs;

namespace PostService.Application.Interfaces
{
    public interface ICommentService
    {
        Task<List<GetCommentDTO>> GetCommentsAsync();

        Task<GetCommentDTO?> GetCommentByIdAsync(Guid id);

        Task AddCommentAsync(AddCommentDTO addCommentDTO);

        Task<bool> UpdateCommentAsync(UpdateCommentDTO updateCommentDTO);

        Task<bool> RemoveCommentByIdAsync(Guid id);
    }
}
