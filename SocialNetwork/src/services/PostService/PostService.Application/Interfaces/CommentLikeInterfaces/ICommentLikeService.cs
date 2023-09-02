using PostService.Application.DTOs.CommentLikeDTOs;

namespace PostService.Application.Interfaces.CommentsUserProfileInterfaces
{
    public interface ICommentLikeService
    {
        Task<GetCommentLikeDTO> AddCommentLikeAsync(AddCommentLikeDTO addCommentLikeDTO);
        
        Task RemoveCommentLikeByIdAsync(Guid id);
    }
}
