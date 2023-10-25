using ChatService.Infrastructure.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace ChatService.Infrastructure.Services
{
    public class KafkaConsumerService<TOperation, TData> : BackgroundService
    {
        private readonly KafkaConsumerConfig<TOperation, TData> _config;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public KafkaConsumerService(IOptions<KafkaConsumerConfig<TOperation, TData>> config,
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

                try
                {
                    var result = consumer.Consume(TimeSpan.FromMilliseconds(15));

                    if (result != null)
                    {
                        var operation = JsonSerializer.Deserialize<TOperation>(result.Message.Key)!;
                        var data = JsonSerializer.Deserialize<TData>(result.Message.Value)!;
                        await _handler.HandleAsync(operation, data);

                        consumer.Commit(result);
                        consumer.StoreOffset(result);
                    }
                }
                catch { }
            }
        }
    }
}
