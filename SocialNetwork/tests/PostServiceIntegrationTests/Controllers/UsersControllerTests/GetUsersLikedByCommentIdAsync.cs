﻿using FluentAssertions.Execution;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PostService.Infrastructure.Data;
using PostServiceIntegrationTests.FakeDataGenerators;
using System.Net;
using Testcontainers.Kafka;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace PostServiceIntegrationTests.Controllers.UsersControllerTests
{
    public class GetUsersLikedByCommentIdAsync
    {
        private readonly HttpClient _httpClient;
        private readonly FakeUsersGenerator _fakeUsersGenerator;
        private readonly FakePostsGenerator _fakePostsGenerator;
        private readonly FakeCommentsGenerator _fakeCommentsGenerator;
        private readonly FakePostLikesGenerator _fakePostLikesGenerator;

        public GetUsersLikedByCommentIdAsync()
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

            _fakePostLikesGenerator = new FakePostLikesGenerator();
            _fakePostLikesGenerator.InitializeData(_fakeUsersGenerator.Users, _fakePostsGenerator.Posts);
            dataContext.AddRange(_fakePostLikesGenerator.PostLikes);

            dataContext.SaveChanges();

            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task GetUsersLikedByCommentIdAsyncTestReturnsNotFound()
        {
            var commentId = Guid.NewGuid();

            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/api/comments/{commentId}/likes/users");
            var response = await _httpClient.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetUsersLikedByCommentIdAsyncTestReturnsOK()
        {
            var commentId = _fakeCommentsGenerator.Comments.First().Id;

            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/api/comments/{commentId}/likes/users");
            var response = await _httpClient.SendAsync(request);

            using (new AssertionScope())
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);
            }
        }
    }
}