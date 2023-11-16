using Microsoft.Extensions.DependencyInjection;
using PostService.Infrastructure.Data;
using PostServiceIntegrationTests.FakeDataGenerators;
using Testcontainers.Kafka;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace PostServiceIntegrationTests.Controllers
{
    public abstract class ControllerTests
    {
        protected readonly HttpClient _httpClient;
        protected readonly FakeUsersGenerator _fakeUsersGenerator;
        protected readonly FakePostsGenerator _fakePostsGenerator;
        protected readonly FakeCommentsGenerator _fakeCommentsGenerator;
        protected readonly FakePostLikesGenerator _fakePostLikesGenerator;
        protected readonly FakeCommentLikesGenerator _fakeCommentLikesGenerator;

        public ControllerTests()
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

            _fakeUsersGenerator = new FakeUsersGenerator();
            _fakePostsGenerator = new FakePostsGenerator();
            _fakeCommentsGenerator = new FakeCommentsGenerator();
            _fakePostLikesGenerator = new FakePostLikesGenerator();
            _fakeCommentLikesGenerator = new FakeCommentLikesGenerator();

            var scope = factory.Services.CreateScope();
            var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            InitializeDatabase(dataContext);

            _httpClient = factory.CreateClient();
        }

        protected abstract void InitializeDatabase(DataContext dataContext);
    }
}
