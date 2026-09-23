using System.Text.Json;
using Confluent.Kafka;
using CrudDemoPro.Models;
using Microsoft.Extensions.Options;

namespace CrudDemoPro.Infrastructure;

public interface IKafkaEventPublisher
{
    Task PublishAsync(string eventType, Usuario usuario, CancellationToken cancellationToken = default);
}

public sealed class KafkaEventPublisher : IKafkaEventPublisher, IDisposable
{
    private readonly KafkaOptions _options;
    private readonly IProducer<string, string>? _producer;

    public KafkaEventPublisher(IOptions<KafkaOptions> options)
    {
        _options = options.Value;

        if (_options.Enabled)
        {
            if (string.IsNullOrWhiteSpace(_options.BootstrapServers))
                throw new InvalidOperationException("Kafka:BootstrapServers must be configured when Kafka is enabled.");
            if (string.IsNullOrWhiteSpace(_options.Topic))
                throw new InvalidOperationException("Kafka:Topic must be configured when Kafka is enabled.");

            _producer = new ProducerBuilder<string, string>(
                new ProducerConfig { BootstrapServers = _options.BootstrapServers }).Build();
        }
    }

    public async Task PublishAsync(string eventType, Usuario usuario, CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
            return;

        var message = new
        {
            EventType = eventType,
            OccurredAtUtc = DateTime.UtcNow,
            Data = usuario
        };

        await _producer!.ProduceAsync(_options.Topic, new Message<string, string>
        {
            Key = usuario.Id.ToString(),
            Value = JsonSerializer.Serialize(message)
        }, cancellationToken);
    }

    public void Dispose()
    {
        if (_producer is null)
            return;

        _producer.Flush(TimeSpan.FromSeconds(5));
        _producer.Dispose();
    }
}
