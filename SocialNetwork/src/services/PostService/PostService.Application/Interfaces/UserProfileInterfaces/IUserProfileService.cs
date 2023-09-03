using PostService.Application.DTOs.UserProfileDTOs;

namespace PostService.Application.Interfaces.UserProfileInterfaces
{
    public interface IUserProfileService
    {
        Task<List<GetUserDTO>> GetUserProfilesLikedByCommentIdAsync(Guid commentId);
    }
}
