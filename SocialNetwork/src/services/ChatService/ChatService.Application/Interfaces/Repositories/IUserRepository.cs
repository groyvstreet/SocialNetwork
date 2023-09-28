using ChatService.Domain.Entities;

namespace ChatService.Application.Interfaces.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task UpdateAsync(User user);
    }
}
