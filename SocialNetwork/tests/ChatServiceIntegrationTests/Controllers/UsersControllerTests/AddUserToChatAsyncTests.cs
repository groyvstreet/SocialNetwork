﻿using ChatService.Application.DTOs.ChatDTOs;
using ChatService.Domain.Entities;
using ChatServiceIntegrationTests.FakeDataGenerators;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Text;
using Testcontainers.Kafka;
using Testcontainers.MongoDb;
using Testcontainers.Redis;

namespace ChatServiceIntegrationTests.Controllers.UsersControllerTests
{
    public class AddUserToChatAsyncTests
    {
        private readonly HttpClient _httpClient;
        private readonly FakeUsersGenerator _fakeUsersGenerator;
        private readonly FakeMessagesGenerator _fakeMessagesGenerator;
        private readonly FakeChatsGenerator _fakeChatsGenerator;

        public AddUserToChatAsyncTests()
        {
            var mongoDbContainer = new MongoDbBuilder().Build();
            var mongoDbContainerTask = mongoDbContainer.StartAsync();

            var redisContainer = new RedisBuilder().Build();
            var redisContainerTask = redisContainer.StartAsync();

            var kafkaContainer = new KafkaBuilder().Build();
            var kafkaContainerTask = kafkaContainer.StartAsync();

            mongoDbContainerTask.Wait();
            redisContainerTask.Wait();
            kafkaContainerTask.Wait();

            var factory = new CustomWebApplicationFactory<Program>(mongoDbContainer.GetConnectionString(),
                redisContainer.GetConnectionString(),
                kafkaContainer.GetBootstrapAddress());

            var scope = factory.Services.CreateScope();
            var mongoDatabase = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();

            _fakeUsersGenerator = new FakeUsersGenerator();
            _fakeUsersGenerator.InitializeData();
            var usersCollection = mongoDatabase.GetCollection<User>("users");
            usersCollection.InsertMany(_fakeUsersGenerator.Users);

            _fakeMessagesGenerator = new FakeMessagesGenerator();

            foreach (var user in _fakeUsersGenerator.Users)
            {
                _fakeMessagesGenerator.InitializeData(user);
            }

            _fakeChatsGenerator = new FakeChatsGenerator();

            for (var i = 0; i < _fakeUsersGenerator.Users.Count / 2; i++)
            {
                var users = _fakeUsersGenerator.Users.Skip(i * 2).Take(2).ToList();
                var messages = _fakeMessagesGenerator.Messages.Skip(i * 2).Take(2).ToList();

                _fakeChatsGenerator.InitializeData(users, messages);
            }

            var chatsCollection = mongoDatabase.GetCollection<Chat>("chats");
            chatsCollection.InsertMany(_fakeChatsGenerator.Chats);

            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task AddUserToChatAsyncTestReturnsForbidden()
        {
            var chatId = _fakeChatsGenerator.Chats.First().Id;
            var userId = _fakeUsersGenerator.Users.First().Id;
            var invitedUserId = _fakeUsersGenerator.Users.Last().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var addUserToChatDTO = new AddUserToChatDTO
            {
                ChatId = chatId,
                UserId = userId,
                InvitedUserId = invitedUserId
            };

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/chats/users/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(addUserToChatDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task AddUserToChatAsyncTestReturnsForbidden2()
        {
            var chatId = _fakeChatsGenerator.Chats.First().Id;
            var userId = _fakeUsersGenerator.Users[4].Id;
            var invitedUserId = _fakeUsersGenerator.Users.Last().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var addUserToChatDTO = new AddUserToChatDTO
            {
                ChatId = chatId,
                UserId = userId,
                InvitedUserId = invitedUserId
            };

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/chats/users/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(addUserToChatDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task AddUserToChatAsyncTestReturnsNotFound()
        {
            var chatId = _fakeChatsGenerator.Chats.First().Id;
            var userId = _fakeUsersGenerator.Users.First().Id;
            var invitedUserId = Guid.NewGuid();
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var addUserToChatDTO = new AddUserToChatDTO
            {
                ChatId = chatId,
                UserId = userId,
                InvitedUserId = invitedUserId
            };

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/chats/users/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(addUserToChatDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task AddUserToChatAsyncTestReturnsNoContent()
        {
            var chatId = _fakeChatsGenerator.Chats.First().Id;
            var userId = _fakeUsersGenerator.Users.First().Id;
            var invitedUserId = _fakeUsersGenerator.Users.Last().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var addUserToChatDTO = new AddUserToChatDTO
            {
                ChatId = chatId,
                UserId = userId,
                InvitedUserId = invitedUserId
            };

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/chats/users/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(addUserToChatDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}