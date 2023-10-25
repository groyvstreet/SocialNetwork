using ChatService.Application.Interfaces.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace ChatService.Infrastructure.CacheRepositories
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
            var json = await _distributedCache.GetStringAsync($"{nameof(T)}-{key}");

            if (json is null)
            {
                return null;
            }

            var entity = JsonSerializer.Deserialize<T>(json);

            return entity;
        }

        public async Task SetAsync(string key, T post)
        {
            var json = JsonSerializer.Serialize(post);
            await _distributedCache.SetStringAsync($"{nameof(T)}-{key}", json, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3)
            });
        }

        public async Task RemoveAsync(string key)
        {
            await _distributedCache.RemoveAsync($"{nameof(T)}-{key}");
        }
    }
}
