using Confluent.Kafka;

namespace PostService.Infrastructure
{
    public class KafkaConsumerConfig<TOperation, TData> : ConsumerConfig
    {
        public string Topic { get; set; } = string.Empty;
    }
}
