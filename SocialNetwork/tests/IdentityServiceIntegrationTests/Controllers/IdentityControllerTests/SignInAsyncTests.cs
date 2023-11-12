using DotNet.Testcontainers.Builders;
using FluentAssertions;
using IdentityService.DAL.Entities;
using IdentityServiceIntegrationTests.FakeDataGenerators;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Testcontainers.Kafka;
using Testcontainers.Redis;

namespace IdentityServiceIntegrationTests.Controllers.IdentityControllerTests
{
    public class SignInAsyncTests
    {
        private readonly HttpClient _httpClient;
        private readonly FakeUsersGenerator _fakeUsersGenerator;

        public SignInAsyncTests()
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

            _fakeUsersGenerator = new FakeUsersGenerator();
            _fakeUsersGenerator.InitializeData();
            _fakeUsersGenerator.Users.ForEach(user => userManager.CreateAsync(user, "string").Wait());

            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task SignInAsyncTestReturnsOK()
        {
            var user = _fakeUsersGenerator.Users.First();

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/identity/signin?email={user.Email}&password=string");
            var response = await _httpClient.SendAsync(request);
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task SignInAsyncTestReturnsUnauthorized()
        {
            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/identity/signin?email=string&password=string");
            var response = await _httpClient.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
