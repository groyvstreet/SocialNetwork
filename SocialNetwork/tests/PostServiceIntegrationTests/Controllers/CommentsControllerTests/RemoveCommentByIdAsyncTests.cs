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

namespace PostServiceIntegrationTests.Controllers.CommentsControllerTests
{
    public class RemoveCommentByIdAsyncTests
    {
        private readonly HttpClient _httpClient;
        private readonly FakeUsersGenerator _fakeUsersGenerator;
        private readonly FakePostsGenerator _fakePostsGenerator;
        private readonly FakeCommentsGenerator _fakeCommentsGenerator;

        public RemoveCommentByIdAsyncTests()
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

            _fakeCommentsGenerator = new FakeCommentsGenerator();
            _fakeCommentsGenerator.InitializeData(_fakeUsersGenerator.Users, _fakePostsGenerator.Posts);
            dataContext.AddRange(_fakeCommentsGenerator.Comments);

            dataContext.SaveChanges();

            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task RemoveCommentByIdAsyncTestReturnsUnauthorized()
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod("DELETE"), $"/api/comments/{Guid.NewGuid()}");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task RemoveCommentByIdAsyncTestReturnsForbidden()
        {
            // Arrange
            var commentId = _fakeCommentsGenerator.Comments.First().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var request = new HttpRequestMessage(new HttpMethod("DELETE"), $"/api/comments/{commentId}");
            request.Headers.Add("Authorization", $"Bearer {token}");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task RemoveCommentByIdAsyncTestReturnsNotFound()
        {
            // Arrange
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var request = new HttpRequestMessage(new HttpMethod("DELETE"), $"/api/comments/{Guid.NewGuid()}");
            request.Headers.Add("Authorization", $"Bearer {token}");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task RemoveCommentByIdAsyncTestReturnsNoContent()
        {
            // Arrange
            var commentId = _fakeCommentsGenerator.Comments.First().Id;
            var userId = _fakeUsersGenerator.Users.First().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var request = new HttpRequestMessage(new HttpMethod("DELETE"), $"/api/comments/{commentId}");
            request.Headers.Add("Authorization", $"Bearer {token}");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
