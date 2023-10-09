using PostService.Application.DTOs.CommentLikeDTOs;

namespace PostService.Application.Interfaces.CommentLikeInterfaces
{
    public interface ICommentLikeService
    {
        Task<GetCommentLikeDTO> AddCommentLikeAsync(AddRemoveCommentLikeDTO addRemoveCommentLikeDTO, Guid authenticatedUserId);
        
        Task RemoveCommentLikeAsync(AddRemoveCommentLikeDTO addRemoveCommentLikeDTO, Guid authenticatedUserId);
    }
}
