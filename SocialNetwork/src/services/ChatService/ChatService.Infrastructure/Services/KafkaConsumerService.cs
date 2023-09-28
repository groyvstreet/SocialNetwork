using ChatService.Application;
using ChatService.Application.Interfaces.Repositories;
using ChatService.Domain.Entities;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace ChatService.Infrastructure.Services
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly string _bootstrapServers;
        private readonly string _groupId = "chat_group";
        private readonly IUserRepository _userRepository;
        private readonly IDialogRepository _dialogRepository;
        private readonly IChatRepository _chatRepository;

        public KafkaConsumerService(IOptions<KafkaOptions> kafkaOptions, IServiceProvider serviceProvider)
        {
            _bootstrapServers = kafkaOptions.Value.BootstrapServers;

            var scope = serviceProvider.CreateScope();
            _userRepository = scope.ServiceProvider.GetService<IUserRepository>()!;
            _dialogRepository = scope.ServiceProvider.GetService<IDialogRepository>()!;
            _chatRepository = scope.ServiceProvider.GetService<IChatRepository>()!;
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
                    await _userRepository.UpdateAsync(request.Data);
                    await _dialogRepository.UpdateUserAsync(request.Data);
                    await _chatRepository.UpdateUserAsync(request.Data);
                    break;
                case RequestOperation.Remove:
                    await _userRepository.RemoveAsync(request.Data);
                    await _dialogRepository.RemoveUserAsync(request.Data);
                    await _chatRepository.RemoveUserAsync(request.Data);
                    break;
                default:
                    break;
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _ = Task.Run(async () => await SubscribeOnTopic<User>("users", UserRequestHandle));

            return Task.CompletedTask;
        }
    }
}
