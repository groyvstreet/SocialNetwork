﻿using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using PostService.Application.DTOs.CommentLikeDTOs;
using PostService.Infrastructure.Data;
using PostServiceIntegrationTests.FakeDataGenerators;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Testcontainers.Kafka;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace PostServiceIntegrationTests.Controllers.CommentLikesControllerTests
{
    public class AddCommentLikeAsyncTests
    {
        private readonly HttpClient _httpClient;
        private readonly FakeUsersGenerator _fakeUsersGenerator;
        private readonly FakePostsGenerator _fakePostsGenerator;
        private readonly FakeCommentsGenerator _fakeCommentsGenerator;
        private readonly FakeCommentLikesGenerator _fakeCommentLikesGenerator;

        public AddCommentLikeAsyncTests()
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

            _fakeCommentLikesGenerator = new FakeCommentLikesGenerator();
            _fakeCommentLikesGenerator.InitializeData(_fakeUsersGenerator.Users.TakeLast(1).ToList(), _fakeCommentsGenerator.Comments);
            dataContext.AddRange(_fakeCommentLikesGenerator.CommentLikes);

            dataContext.SaveChanges();

            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task AddCommentLikeAsyncTestReturnsUnauthorized()
        {
            var addRemoveCommentLikeDTO = new AddRemoveCommentLikeDTO();

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/comment-likes/");
            var body = JsonSerializer.Serialize(addRemoveCommentLikeDTO);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task AddCommentLikeAsyncTestReturnsForbidden()
        {
            var commentId = _fakeCommentsGenerator.Comments.First().Id;
            var userId = _fakeUsersGenerator.Users.First().Id;

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var addRemoveCommentLikeDTO = new AddRemoveCommentLikeDTO()
            {
                CommentId = commentId,
                UserId = userId
            };

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/comment-likes/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(addRemoveCommentLikeDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task AddCommentLikeAsyncTestReturnsNotFound()
        {
            var commentId = Guid.NewGuid();
            var userId = _fakeUsersGenerator.Users.First().Id;

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var addRemoveCommentLikeDTO = new AddRemoveCommentLikeDTO()
            {
                CommentId = commentId,
                UserId = userId
            };

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/comment-likes/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(addRemoveCommentLikeDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task AddCommentLikeAsyncTestReturnsConflict()
        {
            var commentId = _fakeCommentsGenerator.Comments.First().Id;
            var userId = _fakeUsersGenerator.Users.Last().Id;

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var addRemoveCommentLikeDTO = new AddRemoveCommentLikeDTO()
            {
                CommentId = commentId,
                UserId = userId
            };

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/comment-likes/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(addRemoveCommentLikeDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task AddCommentLikeAsyncTestReturnsOK()
        {
            var commentId = _fakeCommentsGenerator.Comments.First().Id;
            var userId = _fakeUsersGenerator.Users.First().Id;

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var addRemoveCommentLikeDTO = new AddRemoveCommentLikeDTO()
            {
                CommentId = commentId,
                UserId = userId
            };

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/comment-likes/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(addRemoveCommentLikeDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);

            using (new AssertionScope())
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var commentLikeJson = await response.Content.ReadAsStringAsync();
                var commentLike = JsonSerializer.Deserialize<GetCommentLikeDTO>(commentLikeJson, jsonSerializerOptions)!;
                commentLike.UserId.Should().Be(addRemoveCommentLikeDTO.UserId);
                commentLike.CommentId.Should().Be(addRemoveCommentLikeDTO.CommentId);
            }
        }
    }
}