using ChatService.Domain.Entities;
using ChatServiceIntegrationTests.FakeDataGenerators;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Testcontainers.Kafka;
using Testcontainers.MongoDb;
using Testcontainers.Redis;

namespace ChatServiceIntegrationTests.Controllers
{
    public abstract class ControllerTests
    {
        protected readonly HttpClient _httpClient;
        protected readonly FakeUsersGenerator _fakeUsersGenerator;
        protected readonly FakeMessagesGenerator _fakeMessagesGenerator;
        protected readonly FakeDialogsGenerator _fakeDialogsGenerator;
        protected readonly FakeChatsGenerator _fakeChatsGenerator;

        public ControllerTests()
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

            _fakeUsersGenerator = new FakeUsersGenerator();
            _fakeMessagesGenerator = new FakeMessagesGenerator();
            _fakeDialogsGenerator = new FakeDialogsGenerator();
            _fakeChatsGenerator = new FakeChatsGenerator();

            var scope = factory.Services.CreateScope();
            var mongoDatabase = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();

            InitializeDatabase(mongoDatabase);

            _httpClient = factory.CreateClient();
        }

        private void InitializeDatabase(IMongoDatabase mongoDatabase)
        {
            _fakeUsersGenerator.InitializeData();
            var usersCollection = mongoDatabase.GetCollection<User>("users");
            usersCollection.InsertMany(_fakeUsersGenerator.Users);

            foreach (var user in _fakeUsersGenerator.Users)
            {
                _fakeMessagesGenerator.InitializeData(user);
            }

            for (var i = 0; i < _fakeUsersGenerator.Users.Count / 2; i++)
            {
                var users = _fakeUsersGenerator.Users.Skip(i * 2).Take(2).ToList();
                var messages = _fakeMessagesGenerator.Messages.Skip(i * 2).Take(2).ToList();

                _fakeDialogsGenerator.InitializeData(users, messages);
            }

            var dialogsCollection = mongoDatabase.GetCollection<Dialog>("dialogs");
            dialogsCollection.InsertMany(_fakeDialogsGenerator.Dialogs);

            for (var i = 0; i < _fakeUsersGenerator.Users.Count / 2; i++)
            {
                var users = _fakeUsersGenerator.Users.Skip(i * 2).Take(2).ToList();
                var messages = _fakeMessagesGenerator.Messages.Skip(i * 2).Take(2).ToList();

                _fakeChatsGenerator.InitializeData(users, messages);
            }

            var chatsCollection = mongoDatabase.GetCollection<Chat>("chats");
            chatsCollection.InsertMany(_fakeChatsGenerator.Chats);
        }
    }
}
