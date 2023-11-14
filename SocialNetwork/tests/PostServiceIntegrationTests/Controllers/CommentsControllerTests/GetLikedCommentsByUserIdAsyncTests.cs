using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using PostService.Application.DTOs.CommentDTOs;
using PostService.Infrastructure.Data;
using PostServiceIntegrationTests.FakeDataGenerators;
using System.Net;
using System.Text.Json;
using Testcontainers.Kafka;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace PostServiceIntegrationTests.Controllers.CommentsControllerTests
{
    public class GetLikedCommentsByUserIdAsyncTests
    {
        private readonly HttpClient _httpClient;
        private readonly FakeUsersGenerator _fakeUsersGenerator;
        private readonly FakePostsGenerator _fakePostsGenerator;
        private readonly FakeCommentsGenerator _fakeCommentsGenerator;
        private readonly FakeCommentLikesGenerator _fakeCommentLikesGenerator;

        public GetLikedCommentsByUserIdAsyncTests()
        {
            var postgreSqlContainer = new PostgreSqlBuilder().Build();
            var postgreSqlContainerTask = postgreSqlContainer.StartAsync();
            postgreSqlContainerTask.Wait();

            var redisContainer = new RedisBuilder().Build();
            var redisContainerTask = redisContainer.StartAsync();
            redisContainerTask.Wait();

            var kafkaContainer = new KafkaBuilder().Build();
            var kafkaContainerTask = kafkaContainer.StartAsync();
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

            _fakeCommentLikesGenerator = new FakeCommentLikesGenerator();
            _fakeCommentLikesGenerator.InitializeData(_fakeUsersGenerator.Users, _fakeCommentsGenerator.Comments);
            dataContext.AddRange(_fakeCommentLikesGenerator.CommentLikes);

            dataContext.SaveChanges();

            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task GetLikedCommentsByUserIdAsyncTestReturnsNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/api/users/{userId}/comment-likes/comments");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetLikedCommentsByUserIdAsyncTestReturnsOK()
        {
            // Arrange
            var userId = _fakeUsersGenerator.Users.First().Id;

            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/api/users/{userId}/comment-likes/comments");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            using (new AssertionScope())
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var commentsJson = await response.Content.ReadAsStringAsync();
                var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var comments = JsonSerializer.Deserialize<List<GetCommentDTO>>(commentsJson, jsonSerializerOptions)!;
                comments.Should().Contain(comment => comment.UserId == userId);
            }
        }
    }
}
