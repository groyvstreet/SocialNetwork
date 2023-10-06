using Confluent.Kafka;

namespace IdentityService.BLL
{
    public class KafkaProducerConfig<TOperation, TData> : ProducerConfig
    {
        public string Topic { get; set; } = string.Empty;
    }
}
