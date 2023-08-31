using System.Linq.Expressions;

namespace PostService.Application.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAsync();

        Task<List<T>> GetByAsync(Expression<Func<T, bool>> predicate);

        Task<T?> GetFirstOrDefaultByAsync(Expression<Func<T, bool>> predicate);

        Task AddAsync(T entity);

        Task UpdateAsync(T entity);

        Task RemoveAsync(T entity);
    }
}
