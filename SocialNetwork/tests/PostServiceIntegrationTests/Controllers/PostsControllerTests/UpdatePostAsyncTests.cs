using FluentAssertions.Execution;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PostService.Application.DTOs.PostDTOs;
using PostService.Infrastructure.Data;
using PostServiceIntegrationTests.FakeDataGenerators;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Text;
using Testcontainers.Kafka;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace PostServiceIntegrationTests.Controllers.PostsControllerTests
{
    public class UpdatePostAsyncTests
    {
        private readonly HttpClient _httpClient;
        private readonly FakeUsersGenerator _fakeUsersGenerator;
        private readonly FakePostsGenerator _fakePostsGenerator;

        public UpdatePostAsyncTests()
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
        public async Task UpdatePostAsyncTestReturnsUnauthorized()
        {
            // Arrange
            var updatePostDTO = new UpdatePostDTO();

            var request = new HttpRequestMessage(new HttpMethod("PUT"), $"/api/posts/");
            var body = JsonSerializer.Serialize(updatePostDTO);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task UpdatePostAsyncTestReturnsForbidden()
        {
            // Arrange
            var postId = _fakePostsGenerator.Posts.First().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var updatePostDTO = new UpdatePostDTO
            {
                Id = postId,
                Text = "text"
            };

            var request = new HttpRequestMessage(new HttpMethod("PUT"), $"/api/posts/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(updatePostDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task UpdatePostAsyncTestReturnsBadRequest()
        {
            // Arrange
            var postId = _fakePostsGenerator.Posts.First().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var updatePostDTO = new UpdatePostDTO();

            var request = new HttpRequestMessage(new HttpMethod("PUT"), $"/api/posts/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(updatePostDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdatePostAsyncTestReturnsNotFound()
        {
            // Arrange
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var updatePostDTO = new UpdatePostDTO
            {
                Id = Guid.NewGuid(),
                Text = "text"
            };

            var request = new HttpRequestMessage(new HttpMethod("PUT"), $"/api/posts/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(updatePostDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdatePostAsyncTestReturnsOK()
        {
            // Arrange
            var postId = _fakePostsGenerator.Posts.First().Id;
            var userId = _fakeUsersGenerator.Users.First().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var updatePostDTO = new UpdatePostDTO
            {
                Id = postId,
                Text = "text"
            };

            var request = new HttpRequestMessage(new HttpMethod("PUT"), $"/api/posts/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(updatePostDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            using (new AssertionScope())
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var postJson = await response.Content.ReadAsStringAsync();
                var post = JsonSerializer.Deserialize<GetPostDTO>(postJson, jsonSerializerOptions)!;
                post.Id.Should().Be(postId);
                post.Text.Should().Be(updatePostDTO.Text);
                post.UserId.Should().Be(userId);
            }
        }
    }
}
