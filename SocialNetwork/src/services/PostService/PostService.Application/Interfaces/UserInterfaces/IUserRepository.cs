using PostService.Domain.Entities;

namespace PostService.Application.Interfaces.UserInterfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> GetUserWithPostsByIdAsync(Guid id);
    }
}
