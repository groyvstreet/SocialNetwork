using PostService.Application.DTOs.CommentDTOs;

namespace PostService.Application.Interfaces.CommentInterfaces
{
    public interface ICommentService
    {
        Task<List<GetCommentDTO>> GetCommentsAsync();

        Task<GetCommentDTO> GetCommentByIdAsync(Guid id);

        Task<GetCommentDTO> AddCommentAsync(AddCommentDTO addCommentDTO);

        Task<GetCommentDTO> UpdateCommentAsync(UpdateCommentDTO updateCommentDTO);

        Task RemoveCommentByIdAsync(Guid id);
    }
}
