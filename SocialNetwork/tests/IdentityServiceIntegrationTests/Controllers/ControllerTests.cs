using DotNet.Testcontainers.Builders;
using IdentityService.DAL.Entities;
using IdentityServiceIntegrationTests.FakeDataGenerators;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.Kafka;
using Testcontainers.Redis;

namespace IdentityServiceIntegrationTests.Controllers
{
    public class ControllerTests
    {
        protected readonly HttpClient _httpClient;
        protected readonly FakeUsersGenerator _fakeUsersGenerator;

        public ControllerTests()
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

            _fakeUsersGenerator = new FakeUsersGenerator();

            var scope = factory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            InitializeDatabase(userManager);

            _httpClient = factory.CreateClient();
        }

        private void InitializeDatabase(UserManager<User> userManager)
        {
            _fakeUsersGenerator.InitializeData();
            _fakeUsersGenerator.Users.ForEach(user => userManager.CreateAsync(user, "string").Wait());
        }
    }
}
