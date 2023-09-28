﻿using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PostService.Application;
using PostService.Application.Interfaces.UserInterfaces;
using PostService.Domain.Entities;
using System.Text.Json;

namespace PostService.Infrastructure.Services
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly string _bootstrapServers;
        private readonly string _groupId = "post_group";
        private readonly IUserRepository _userRepository;
        
        public KafkaConsumerService(IOptions<KafkaOptions> kafkaOptions, IServiceProvider serviceProvider)
        {
            _bootstrapServers = kafkaOptions.Value.BootstrapServers;

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
                    userForUpdate!.FirstName = request.Data.FirstName;
                    userForUpdate.LastName = request.Data.LastName;
                    userForUpdate.BirthDate = request.Data.BirthDate;
                    userForUpdate.Image = request.Data.Image;
                    break;
                case RequestOperation.Remove:
                    var userForRemove = await _userRepository.GetFirstOrDefaultByAsync(u => u.Id == request.Data.Id);
                    _userRepository.Remove(userForRemove!);
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
        }
    }
}
