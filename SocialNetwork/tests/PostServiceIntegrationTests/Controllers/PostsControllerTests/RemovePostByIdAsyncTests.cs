using FluentAssertions.Execution;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PostServiceIntegrationTests.FakeDataGenerators;
using System.Net;
using System.Security.Claims;
using Testcontainers.Kafka;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using PostService.Infrastructure.Data;

namespace PostServiceIntegrationTests.Controllers.PostsControllerTests
{
    public class RemovePostByIdAsyncTests
    {
        private readonly HttpClient _httpClient;
        private readonly FakeUsersGenerator _fakeUsersGenerator;
        private readonly FakePostsGenerator _fakePostsGenerator;

        public RemovePostByIdAsyncTests()
        {
            var postgreSqlContainer = new PostgreSqlBuilder().Build();
            var postgreSqlContainerTask = postgreSqlContainer.StartAsync();

            var redisContainer = new RedisBuilder().Build();
            var redisContainerTask = redisContainer.StartAsync();

            var kafkaContainer = new KafkaBuilder().Build();
            var kafkaContainerTask = kafkaContainer.StartAsync();

            postgreSqlContainerTask.Wait();
            redisContainerTask.Wait();
            kafkaContainerTask.Wait();

            var factory = new CustomWebApplicationFactory<Program>(postgreSqlContainer.GetConnectionString(),
                redisContainer.GetConnectionString(),
                kafkaContainer.GetBootstrapAddress());

            var scope = factory.Services.CreateScope();
            var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            _fakeUsersGenerator = new FakeUsersGenerator();
            _fakeUsersGenerator.InitializeData();
            dataContext.AddRange(_fakeUsersGenerator.Users);

            _fakePostsGenerator = new FakePostsGenerator();
            _fakePostsGenerator.InitializeData(_fakeUsersGenerator.Users);
            dataContext.AddRange(_fakePostsGenerator.Posts);

            dataContext.SaveChanges();

            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task RemovePostByIdAsyncTestReturnsUnauthorized()
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod("DELETE"), $"/api/posts/{Guid.NewGuid()}");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task RemovePostByIdAsyncTestReturnsForbidden()
        {
            // Arrange
            var postId = _fakePostsGenerator.Posts.First().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var request = new HttpRequestMessage(new HttpMethod("DELETE"), $"/api/posts/{postId}");
            request.Headers.Add("Authorization", $"Bearer {token}");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task RemovePostByIdAsyncTestReturnsNotFound()
        {
            // Arrange
            var postId = _fakePostsGenerator.Posts.First().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var request = new HttpRequestMessage(new HttpMethod("DELETE"), $"/api/posts/{Guid.NewGuid()}");
            request.Headers.Add("Authorization", $"Bearer {token}");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task RemovePostByIdAsyncTestReturnsNoContent()
        {
            // Arrange
            var postId = _fakePostsGenerator.Posts.First().Id;
            var userId = _fakeUsersGenerator.Users.First().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var request = new HttpRequestMessage(new HttpMethod("DELETE"), $"/api/posts/{postId}");
            request.Headers.Add("Authorization", $"Bearer {token}");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
