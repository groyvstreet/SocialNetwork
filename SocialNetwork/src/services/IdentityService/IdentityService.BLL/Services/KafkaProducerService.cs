using IdentityService.BLL.Interfaces;
using Confluent.Kafka;
using System.Text.Json;
using IdentityService.BLL.DTOs.UserDTOs;
using Microsoft.Extensions.Options;

namespace IdentityService.BLL.Services
{
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly IProducer<string, string> _producer;

        public KafkaProducerService(IOptions<KafkaOptions> kafkaOptions)
        {
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = kafkaOptions.Value.BootstrapServers
            };
            _producer = new ProducerBuilder<string, string>(producerConfig).Build();
        }

        public async Task SendUserRequestAsync(RequestOperation requestOperation, GetUserDTO user)
        {
            /*var request = new
            {
                Operation = requestOperation,
                Data = user
            };*/
            var message = new Message<string, string>
            {
                Key = JsonSerializer.Serialize(requestOperation),
                Value = JsonSerializer.Serialize(user)
            };
            await _producer.ProduceAsync("users", message);
        }
    }
}
