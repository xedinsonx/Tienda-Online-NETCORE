namespace CrudDemoPro.Infrastructure;

public sealed class KafkaOptions
{
    public const string SectionName = "Kafka";

    public string BootstrapServers { get; set; } = "localhost:9092";
    public string Topic { get; set; } = "usuarios-events";
    public bool Enabled { get; set; } = true;
}
