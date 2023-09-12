using ChatService.Domain.Entities;

namespace ChatService.Application.Interfaces
{
    public interface IBaseRepository<T> where T : IEntity
    {
        Task<List<T>> GetAllAsync();

        Task<T?> GetFirstOrDefaultByIdAsync(Guid id);

        Task AddAsync(T entity);

        Task UpdateAsync(T entity);

        Task RemoveByIdAsync(Guid id);
    }
}
