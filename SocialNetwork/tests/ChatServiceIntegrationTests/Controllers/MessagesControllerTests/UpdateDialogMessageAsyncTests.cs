using ChatService.Application.DTOs.MessageDTOs;
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

namespace ChatServiceIntegrationTests.Controllers.MessagesControllerTests
{
    public class UpdateDialogMessageAsyncTests
    {
        private readonly HttpClient _httpClient;
        private readonly FakeUsersGenerator _fakeUsersGenerator;
        private readonly FakeMessagesGenerator _fakeMessagesGenerator;
        private readonly FakeDialogsGenerator _fakeDialogsGenerator;
        private readonly FakeChatsGenerator _fakeChatsGenerator;

        public UpdateDialogMessageAsyncTests()
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

            _fakeDialogsGenerator = new FakeDialogsGenerator();

            for (var i = 0; i < _fakeUsersGenerator.Users.Count / 2; i++)
            {
                var users = _fakeUsersGenerator.Users.Skip(i * 2).Take(2).ToList();
                var messages = _fakeMessagesGenerator.Messages.Skip(i * 2).Take(2).ToList();

                _fakeDialogsGenerator.InitializeData(users, messages);
            }

            var dialogsCollection = mongoDatabase.GetCollection<Dialog>("dialogs");
            dialogsCollection.InsertMany(_fakeDialogsGenerator.Dialogs);

            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task UpdateDialogMessageAsyncTestReturnsForbidden()
        {
            var dialogId = _fakeDialogsGenerator.Dialogs.First().Id;
            var messageId = _fakeMessagesGenerator.Messages.First().Id;
            var authenticatedUserId = _fakeUsersGenerator.Users.Last().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, authenticatedUserId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var updateDialogMessageDTO = new UpdateDialogMessageDTO
            {
                DialogId = dialogId,
                MessageId = messageId,
                Text = "text"
            };

            var request = new HttpRequestMessage(new HttpMethod("PUT"), $"/api/dialogs/messages");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(updateDialogMessageDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task UpdateDialogMessageAsyncTestReturnsDialogNotFound()
        {
            var dialogId = Guid.NewGuid();
            var messageId = _fakeMessagesGenerator.Messages.First().Id;
            var authenticatedUserId = _fakeUsersGenerator.Users.First().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, authenticatedUserId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var updateDialogMessageDTO = new UpdateDialogMessageDTO
            {
                DialogId = dialogId,
                MessageId = messageId,
                Text = "text"
            };

            var request = new HttpRequestMessage(new HttpMethod("PUT"), $"/api/dialogs/messages");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(updateDialogMessageDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateDialogMessageAsyncTestReturnsMessageNotFound()
        {
            var dialogId = _fakeDialogsGenerator.Dialogs.First().Id;
            var messageId = Guid.NewGuid();
            var authenticatedUserId = _fakeUsersGenerator.Users.First().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, authenticatedUserId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var updateDialogMessageDTO = new UpdateDialogMessageDTO
            {
                DialogId = dialogId,
                MessageId = messageId,
                Text = "text"
            };

            var request = new HttpRequestMessage(new HttpMethod("PUT"), $"/api/dialogs/messages");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(updateDialogMessageDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateDialogMessageAsyncTestReturnsNoContent()
        {
            var dialogId = _fakeDialogsGenerator.Dialogs.First().Id;
            var messageId = _fakeMessagesGenerator.Messages.First().Id;
            var authenticatedUserId = _fakeUsersGenerator.Users.First().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, authenticatedUserId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var updateDialogMessageDTO = new UpdateDialogMessageDTO
            {
                DialogId = dialogId,
                MessageId = messageId,
                Text = "text"
            };

            var request = new HttpRequestMessage(new HttpMethod("PUT"), $"/api/dialogs/messages");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(updateDialogMessageDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
