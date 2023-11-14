using FluentAssertions;
using IdentityService.DAL.CacheRepositories;
using IdentityService.DAL.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System.Text;

namespace IdentityServiceTests.CacheRepositories.CacheRepositoryTests
{
    public class GetAsyncTests
    {
        private readonly Mock<IDistributedCache> _distributedCache;

        public GetAsyncTests()
        {
            _distributedCache = new Mock<IDistributedCache>();
        }

        [Fact]
        public async Task GetAsyncTestReturnsNull()
        {
            await GetAsyncTestHelherReturnsNull<User>();
            await GetAsyncTestHelherReturnsNull<RefreshToken>();
        }

        [Fact]
        public async Task GetAsyncTestReturnsNotNull()
        {
            await GetAsyncTestHelperReturnsNotNull<User>();
            await GetAsyncTestHelperReturnsNotNull<RefreshToken>();
        }

        private async Task GetAsyncTestHelherReturnsNull<T>() where T : class
        {
            // Arrange
            var _cacheRepository = new CacheRepository<T>(_distributedCache.Object);

            _distributedCache.Setup(distributedCache => distributedCache.GetAsync(It.IsAny<string>(), default).Result)
                .Returns((byte[]?)null);

            var id = Guid.NewGuid().ToString();

            // Act
            var entity = await _cacheRepository.GetAsync(id);

            // Assert
            entity.Should().BeNull();
        }

        private async Task GetAsyncTestHelperReturnsNotNull<T>() where T : class
        {
            // Arrange
            var _cacheRepository = new CacheRepository<T>(_distributedCache.Object);

            var json = "{ \"Id\": \"6f9619ff-8b86-d011-b42d-00cf4fc964ff\" }";
            var bytes = Encoding.UTF8.GetBytes(json);

            _distributedCache.Setup(distributedCache =>
                distributedCache.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()).Result)
                .Returns(bytes);

            var id = Guid.NewGuid().ToString();

            // Act
            var entity = await _cacheRepository.GetAsync(id);

            // Assert
            entity.Should().NotBeNull();
        }
    }
}
