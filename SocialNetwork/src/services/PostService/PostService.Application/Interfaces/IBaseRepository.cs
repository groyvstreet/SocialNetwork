using System.Linq.Expressions;

namespace PostService.Application.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();

        Task<List<T>> GetAllByAsync(Expression<Func<T, bool>> predicate);

        Task<T?> GetFirstOrDefaultByAsync(Expression<Func<T, bool>> predicate);

        Task<T?> GetFirstOrDefaultAsNoTrackingByAsync(Expression<Func<T, bool>> predicate);

        Task AddAsync(T entity);

        void Update(T entity);

        void Remove(T entity);

        Task SaveChangesAsync();

        void ClearTrackedEntities();
    }
}
