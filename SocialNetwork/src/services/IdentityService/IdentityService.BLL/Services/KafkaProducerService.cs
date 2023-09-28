using IdentityService.BLL.Interfaces;
using Confluent.Kafka;
using System.Text.Json;
using IdentityService.BLL.DTOs.UserDTOs;
using Microsoft.Extensions.Options;

namespace IdentityService.BLL.Services
{
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly IProducer<Null, string> _producer;

        public KafkaProducerService(IOptions<KafkaOptions> kafkaOptions)
        {
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = kafkaOptions.Value.BootstrapServers
            };
            _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
        }

        public async Task SendUserRequestAsync(RequestOperation operationRequest, GetUserDTO user)
        {
            var request = new
            {
                Operation = operationRequest,
                Data = user
            };
            var message = new Message<Null, string> { Value = JsonSerializer.Serialize(request) };
            await _producer.ProduceAsync("users", message);
        }
    }
}
