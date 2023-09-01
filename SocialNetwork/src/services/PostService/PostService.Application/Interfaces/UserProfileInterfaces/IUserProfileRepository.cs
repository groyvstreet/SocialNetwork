using PostService.Domain.Entities;

namespace PostService.Application.Interfaces.UserProfileInterfaces
{
    public interface IUserProfileRepository
    {
        Task<UserProfile?> GetUserProfileByIdAsync(Guid id);
    }
}
