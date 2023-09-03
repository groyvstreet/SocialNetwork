using PostService.Domain.Entities;

namespace PostService.Application.Interfaces.UserProfileInterfaces
{
    public interface IUserProfileRepository
    {
        Task<User?> GetUserProfileByIdAsync(Guid id);
    }
}
