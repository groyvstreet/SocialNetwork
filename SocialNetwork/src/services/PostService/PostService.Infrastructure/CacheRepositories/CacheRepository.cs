using Microsoft.Extensions.Caching.Distributed;
using PostService.Application.Interfaces;
using System.Text;
using System.Text.Json;

namespace PostService.Infrastructure.CacheRepositories
{
    public class CacheRepository<T> : ICacheRepository<T> where T : class
    {
        private readonly IDistributedCache _distributedCache;

        public CacheRepository(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<T?> GetAsync(string key)
        {
            var bytes = await _distributedCache.GetAsync($"{nameof(T)}-{key}");

            if (bytes is null)
            {
                return null;
            }

            var json = Encoding.UTF8.GetString(bytes);
            var entity = JsonSerializer.Deserialize<T>(json);

            return entity;
        }

        public async Task SetAsync(string key, T post)
        {
            var json = JsonSerializer.Serialize(post);
            var bytes = Encoding.UTF8.GetBytes(json);

            var distributedCacheEntryOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3)
            };

            await _distributedCache.SetAsync($"{nameof(T)}-{key}", bytes, distributedCacheEntryOptions);
        }

        public async Task RemoveAsync(string key)
        {
            await _distributedCache.RemoveAsync($"{nameof(T)}-{key}");
        }
    }
}
