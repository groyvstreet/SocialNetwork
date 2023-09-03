using PostService.Domain.Entities;

namespace PostService.Application.Interfaces.UserInterfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(Guid id);
    }
}
