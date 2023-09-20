using PostService.Application.DTOs.UserDTOs;

namespace PostService.Application.Interfaces.UserInterfaces
{
    public interface IUserService
    {
        Task<List<GetUserDTO>> GetUsersLikedByCommentIdAsync(Guid commentId);
        Task<List<GetUserDTO>> GetUsersLikedByPostIdAsync(Guid postId);
    }
}
