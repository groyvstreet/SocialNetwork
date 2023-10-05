using Confluent.Kafka;

namespace PostService.Infrastructure
{
    public class ConsumerConfigKafka<TOperation, TData> : ConsumerConfig
    {
        public string Topic { get; set; } = string.Empty;
    }
}
