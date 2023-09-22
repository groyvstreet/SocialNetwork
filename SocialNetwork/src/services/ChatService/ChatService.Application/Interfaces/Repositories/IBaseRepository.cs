using ChatService.Domain.Entities;
using System.Linq.Expressions;

namespace ChatService.Application.Interfaces.Repositories
{
    public interface IBaseRepository<T> where T : IEntity
    {
        Task<List<T>> GetAllAsync();

        Task<List<T>> GetAllByAsync(Expression<Func<T, bool>> predicate);

        Task<T?> GetFirstOrDefaultByAsync(Expression<Func<T, bool>> predicate);

        Task AddAsync(T entity);

        Task UpdateFieldAsync<K>(T entity, Expression<Func<T, K>> field, K value);

        Task RemoveAsync(T entity);
    }
}
