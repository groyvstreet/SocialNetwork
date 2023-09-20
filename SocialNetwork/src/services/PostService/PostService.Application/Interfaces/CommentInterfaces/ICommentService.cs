using PostService.Application.DTOs.CommentDTOs;

namespace PostService.Application.Interfaces.CommentInterfaces
{
    public interface ICommentService
    {
        Task<List<GetCommentDTO>> GetCommentsAsync();

        Task<GetCommentDTO> GetCommentByIdAsync(Guid id);

        Task<List<GetCommentDTO>> GetCommentsByPostIdAsync(Guid postId);

        Task<List<GetCommentDTO>> GetLikedCommentsByUserIdAsync(Guid userProfileId);

        Task<GetCommentDTO> AddCommentAsync(AddCommentDTO addCommentDTO);

        Task<GetCommentDTO> UpdateCommentAsync(UpdateCommentDTO updateCommentDTO);

        Task RemoveCommentByIdAsync(Guid id);
    }
}
