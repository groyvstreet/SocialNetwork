using ChatService.Domain.Entities;
using ChatServiceIntegrationTests.FakeDataGenerators;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Security.Claims;
using System.Text.Json;
using Testcontainers.Kafka;
using Testcontainers.MongoDb;
using Testcontainers.Redis;
using FluentAssertions;
using System.Net;
using FluentAssertions.Execution;
using ChatService.Application.DTOs.DialogDTOs;

namespace ChatServiceIntegrationTests.Controllers.DialogsControllerTests
{
    public class GetDialogsByUserIdAsyncTests
    {
        private readonly HttpClient _httpClient;
        private readonly FakeUsersGenerator _fakeUsersGenerator;
        private readonly FakeMessagesGenerator _fakeMessagesGenerator;
        private readonly FakeDialogsGenerator _fakeDialogsGenerator;

        public GetDialogsByUserIdAsyncTests()
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
        public async Task GetDialogsByUserIdAsyncTestReturnsForbidden()
        {
            var userId = _fakeUsersGenerator.Users.First().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/api/dialogs?userId={userId}");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var response = await _httpClient.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task GetDialogsByUserIdAsyncTestReturnsOK()
        {
            var userId = _fakeUsersGenerator.Users.First().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/api/dialogs?userId={userId}");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var response = await _httpClient.SendAsync(request);

            using (new AssertionScope())
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var dialogsJson = await response.Content.ReadAsStringAsync();
                var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dialogs = JsonSerializer.Deserialize<List<GetDialogDTO>>(dialogsJson, jsonSerializerOptions)!;
                dialogs.Should().Contain(dialog => dialog.Users.Any(user => user.Id == userId));
            }
        }
    }
}
