using IdentityService.BLL.Interfaces;
using Confluent.Kafka;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace IdentityService.BLL.Services
{
    public class KafkaProducerService<TOperation, TData> : IKafkaProducerService<TOperation, TData>
    {
        private readonly KafkaProducerConfig<TOperation, TData> _config;
        private readonly IProducer<string, string> _producer;

        public KafkaProducerService(IOptions<KafkaProducerConfig<TOperation, TData>> config)
        {
            _config = config.Value;
            _producer = new ProducerBuilder<string, string>(_config).Build();
        }

        public async Task SendUserRequestAsync(TOperation requestOperation, TData user)
        {
            var message = new Message<string, string>
            {
                Key = JsonSerializer.Serialize(requestOperation),
                Value = JsonSerializer.Serialize(user)
            };
            await _producer.ProduceAsync(_config.Topic, message);
        }
    }
}
