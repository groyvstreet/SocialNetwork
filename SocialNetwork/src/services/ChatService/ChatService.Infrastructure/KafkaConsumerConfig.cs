using Confluent.Kafka;

namespace ChatService.Infrastructure
{
    public class KafkaConsumerConfig<TOperation, TData> : ConsumerConfig
    {
        public string Topic { get; set; } = string.Empty;
    }
}
