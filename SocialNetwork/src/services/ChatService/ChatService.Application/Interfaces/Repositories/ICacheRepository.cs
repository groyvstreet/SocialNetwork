namespace ChatService.Application.Interfaces.Repositories
{
    public interface ICacheRepository<T> where T : class
    {
        Task<T?> GetAsync(string key);

        Task SetAsync(string key, T value);

        Task RemoveAsync(string key);
    }
}
