using Confluent.Kafka;
using Microsoft.Extensions.Options;
using PetCare.Notification.Application.Kafka;
using PetCare.Notification.Domain.Interfaces;
using System.Text.Json;

namespace PetCare.Notification.Infrastructure.Kafka;

/// <summary>
/// Productor Kafka para publicar eventos de notificación.
/// </summary>
public class NotificationProducer : INotificationProducer
{
    private readonly KafkaSettings _settings;
    private readonly IProducer<string, string> _producer;

    public NotificationProducer(IOptions<KafkaSettings> settings)
    {
        _settings = settings.Value;

        var config = new ProducerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            ClientId = _settings.ClientId,
            MessageTimeoutMs = _settings.MessageTimeoutMs,
            Acks = Acks.All,
            EnableIdempotence = true
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishAsync(string message)
    {
        var kafkaMessage = new Message<string, string>
        {
            Key = Guid.NewGuid().ToString(),
            Value = message
        };

        try
        {
            var deliveryResult = await _producer.ProduceAsync(_settings.Topic, kafkaMessage);

            if (_settings.EnableDebugLogging)
            {
                Console.WriteLine($"✅ Mensaje publicado en Kafka: {JsonSerializer.Serialize(deliveryResult)}");
            }
        }
        catch (ProduceException<string, string> ex)
        {
            Console.WriteLine($"❌ Error al publicar en Kafka: {ex.Error.Reason}");

            if (_settings.EnableDebugLogging)
            {
                Console.WriteLine($"🧪 Detalles: {JsonSerializer.Serialize(ex)}");
            }

            throw;
        }
    }
}
