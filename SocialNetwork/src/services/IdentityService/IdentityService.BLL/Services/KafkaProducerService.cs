using IdentityService.BLL.Interfaces;
using Confluent.Kafka;
using System.Text.Json;
using IdentityService.BLL.DTOs.UserDTOs;

namespace IdentityService.BLL.Services
{
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly IProducer<Null, string> _producer;
        private readonly string _topic;

        public KafkaProducerService(string bootstrapServers, string topic)
        {
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = bootstrapServers
            };
            _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
            _topic = topic;
        }

        public async Task SendUserRequestAsync(UserRequest userRequest, GetUserDTO user)
        {
            var request = new
            {
                Operation = userRequest,
                Data = user
            };
            var message = new Message<Null, string> { Value = JsonSerializer.Serialize(request) };
            await _producer.ProduceAsync(_topic, message);
        }
    }
}
