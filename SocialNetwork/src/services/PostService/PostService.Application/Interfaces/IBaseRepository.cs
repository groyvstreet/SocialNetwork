namespace PostService.Application.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();

        Task<T?> GetFirstOrDefaultByIdAsync(Guid id);

        Task AddAsync(T entity);

        void Remove(T entity);

        Task SaveChangesAsync();
    }
}
