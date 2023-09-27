using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PostService.Application;
using PostService.Application.Interfaces.UserInterfaces;
using System.Text.Json;

namespace PostService.Infrastructure.Services
{
    public class KafkaConsumerService : IHostedService
    {
        private readonly string _bootstrapServers = "kafka:9092";
        private readonly string _topic = "users";
        private readonly string _groupId = "test_group";
        private readonly IUserRepository _userRepository;

        public KafkaConsumerService(IServiceProvider serviceProvider)
        {
            var scope = serviceProvider.CreateScope();
            _userRepository = scope.ServiceProvider.GetService<IUserRepository>()!;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var config = new ConsumerConfig
            {
                GroupId = _groupId,
                BootstrapServers = _bootstrapServers,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _ = Task.Run(async () =>
            {
                using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
                consumer.Subscribe(_topic);

                while (true)
                {
                    var userRepository = _userRepository;

                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    try
                    {
                        var result = consumer.Consume(cancellationToken);
                        var request = JsonSerializer.Deserialize<Request>(result.Message.Value)!;

                        switch (request.Operation)
                        {
                            case UserRequest.Create:
                                await userRepository.AddAsync(request.Data);
                                break;
                            case UserRequest.Update:
                                var user = await userRepository.GetFirstOrDefaultByAsync(u => u.Id == request.Data.Id);
                                user.FirstName = request.Data.FirstName;
                                user.LastName = request.Data.LastName;
                                user.BirthDate = request.Data.BirthDate;
                                user.Image = request.Data.Image;
                                break;
                            case UserRequest.Remove:
                                var userForRemove = await userRepository.GetFirstOrDefaultByAsync(u => u.Id == request.Data.Id);
                                userRepository.Remove(userForRemove);
                                break;
                            default:
                                break;
                        }

                        await userRepository.SaveChangesAsync();
                    }
                    catch { }
                }
            }, cancellationToken);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
