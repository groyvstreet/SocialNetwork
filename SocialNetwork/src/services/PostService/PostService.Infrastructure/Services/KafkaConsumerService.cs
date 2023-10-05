using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PostService.Application;
using PostService.Application.Interfaces.UserInterfaces;
using PostService.Domain.Entities;
using PostService.Infrastructure.Interfaces;
using System.Text.Json;

namespace PostService.Infrastructure.Services
{
    public class KafkaConsumerService<TOperation, TData> : BackgroundService
    {
        /*private readonly string _bootstrapServers;
        private readonly string _groupId;
        private readonly IUserRepository _userRepository;
        
        public KafkaConsumerService(IOptions<KafkaOptions> kafkaOptions, IServiceProvider serviceProvider)
        {
            _bootstrapServers = kafkaOptions.Value.BootstrapServers;
            _groupId = kafkaOptions.Value.GroupId;

            var scope = serviceProvider.CreateScope();
            _userRepository = scope.ServiceProvider.GetService<IUserRepository>()!;
        }

        private async Task SubscribeOnTopic<T>(string topic, Func<Request<T>, Task> handle)
        {
            var config = new ConsumerConfig
            {
                GroupId = _groupId,
                BootstrapServers = _bootstrapServers,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(topic);

            while (true)
            {
                try
                {
                    var result = consumer.Consume();
                    var request = JsonSerializer.Deserialize<Request<T>>(result.Message.Value)!;
                    await handle(request);
                }
                catch { }
            }
        }

        private async Task UserRequestHandle(Request<User> request)
        {
            switch (request.Operation)
            {
                case RequestOperation.Create:
                    await _userRepository.AddAsync(request.Data);
                    break;
                case RequestOperation.Update:
                    var userForUpdate = await _userRepository.GetFirstOrDefaultByAsync(u => u.Id == request.Data.Id);

                    if (userForUpdate is null)
                    {
                        await _userRepository.AddAsync(request.Data);
                    }
                    else
                    {
                        _userRepository.Update(userForUpdate);
                    }

                    break;
                case RequestOperation.Remove:
                    var userForRemove = await _userRepository.GetFirstOrDefaultByAsync(u => u.Id == request.Data.Id);

                    if (userForRemove is not null)
                    {
                        _userRepository.Remove(userForRemove);
                    }

                    break;
                default:
                    break;
            }

            await _userRepository.SaveChangesAsync();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _ = Task.Run(async () => await SubscribeOnTopic<User>("users", UserRequestHandle));

            return Task.CompletedTask;
        }*/

        private readonly ConsumerConfigKafka<TOperation, TData> _config;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        //private IKafkaConsumerHandler<TOperation, TEntity> _handler;

        public KafkaConsumerService(IOptions<ConsumerConfigKafka<TOperation, TData>> config,
                                    IServiceScopeFactory serviceScopeFactory)
        {
            _config = config.Value;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var _handler = scope.ServiceProvider.GetRequiredService<IKafkaConsumerHandler<TOperation, TData>>();

            var builder = new ConsumerBuilder<string, string>(_config);

            using var consumer = builder.Build();
            consumer.Subscribe(_config.Topic);

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(5);
                var result = consumer.Consume(TimeSpan.FromMilliseconds(15));

                if (result != null)
                {
                    Console.WriteLine(result.Message.Value);
                    var operation = JsonSerializer.Deserialize<TOperation>(result.Message.Key)!;
                    var data = JsonSerializer.Deserialize<TData>(result.Message.Value)!;
                    await _handler.HandleAsync(operation, data);

                    //consumer.Commit(result);
                    //consumer.StoreOffset(result);
                }
            }
        }
    }
}
