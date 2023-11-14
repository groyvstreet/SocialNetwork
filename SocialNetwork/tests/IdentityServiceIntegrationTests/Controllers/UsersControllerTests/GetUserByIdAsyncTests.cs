using DotNet.Testcontainers.Builders;
using FluentAssertions;
using FluentAssertions.Execution;
using IdentityService.BLL.DTOs.UserDTOs;
using IdentityService.DAL.Entities;
using IdentityServiceIntegrationTests.FakeDataGenerators;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text.Json;
using Testcontainers.Kafka;
using Testcontainers.Redis;

namespace IdentityServiceIntegrationTests.Controllers.UsersControllerTests
{
    public class GetUserByIdAsyncTests
    {
        private readonly HttpClient _httpClient;

        public GetUserByIdAsyncTests()
        {
            var msSqlServerContainer = new ContainerBuilder()
                .WithName(Guid.NewGuid().ToString("N"))
                .WithImage("mcr.microsoft.com/mssql/server:2019-latest")
                .WithHostname(Guid.NewGuid().ToString("N"))
                .WithExposedPort(1433)
                .WithPortBinding(1433, true)
                .WithEnvironment("SA_PASSWORD", "S3cur3P@ssW0rd!")
                .WithEnvironment("ACCEPT_EULA", "Y")
                .Build();
            var msSqlServerContainerTask = msSqlServerContainer.StartAsync();
            msSqlServerContainerTask.Wait();

            var redisContainer = new RedisBuilder().Build();
            var redisContainerTask = redisContainer.StartAsync();
            redisContainerTask.Wait();

            var kafkaContainer = new KafkaBuilder().Build();
            var kafkaContainerTask = kafkaContainer.StartAsync();
            kafkaContainerTask.Wait();

            var factory = new CustomWebApplicationFactory<Program>(msSqlServerContainer.GetMappedPublicPort(1433),
                redisContainer.GetConnectionString(),
                kafkaContainer.GetBootstrapAddress());

            var scope = factory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            var fakeUsersGenerator = new FakeUsersGenerator();
            fakeUsersGenerator.InitializeData();
            fakeUsersGenerator.Users.ForEach(user => userManager.CreateAsync(user, "string").Wait());

            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task GetUserByIdAsyncTestReturnsHttpOK()
        {
            // Arrange
            var userId = "5";

            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/api/users/{userId}");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            using (new AssertionScope())
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var userJson = await response.Content.ReadAsStringAsync();
                var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var user = JsonSerializer.Deserialize<GetUserDTO>(userJson, jsonSerializerOptions)!;
                user.Id.Should().Be(userId);
            }
        }

        [Fact]
        public async Task GetUserByIdAsyncTestReturnsHttpNotFound()
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/api/users/{Guid.NewGuid()}");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
