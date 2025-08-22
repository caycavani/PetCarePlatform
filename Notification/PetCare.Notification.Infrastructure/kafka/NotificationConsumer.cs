using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PetCare.Notification.Application.DTOs.kafka;
using PetCare.Notification.Infrastructure.kafka.Handlers;
using System.Text.Json;

namespace PetCare.Notification.Infrastructure.Kafka;

public class NotificationConsumer : BackgroundService
{
    private readonly ILogger<NotificationConsumer> _logger;
    private readonly KafkaSettings _settings;
    private readonly IConsumer<string, string> _consumer;
    private readonly Dictionary<string, Func<NotificationEventDto, Task>> _handlers;

    public NotificationConsumer(
        ILogger<NotificationConsumer> logger,
        IOptions<KafkaSettings> kafkaOptions)
    {
        _logger = logger;
        _settings = kafkaOptions.Value;

        var config = new ConsumerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            GroupId = _settings.GroupId,
            ClientId = _settings.ClientId,
            SecurityProtocol = Enum.Parse<SecurityProtocol>(_settings.SecurityProtocol, ignoreCase: true),
            EnableAutoCommit = _settings.EnableAutoCommit,
            SessionTimeoutMs = _settings.SessionTimeoutMs,
            MaxPollIntervalMs = _settings.MaxPollIntervalMs,
            AutoOffsetReset = Enum.Parse<AutoOffsetReset>(_settings.AutoOffsetReset, ignoreCase: true)
        };

        _consumer = new ConsumerBuilder<string, string>(config).Build();

        _handlers = new Dictionary<string, Func<NotificationEventDto, Task>>(StringComparer.OrdinalIgnoreCase)
        {
            { "email", new EmailNotificationHandler().HandleAsync },
            { "sms", new SmsNotificationHandler().HandleAsync },
            { "push", new PushNotificationHandler().HandleAsync }
        };
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(_settings.Topic);

        return Task.Run(() =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer.Consume(stoppingToken);
                    var dto = JsonSerializer.Deserialize<NotificationEventDto>(result.Message.Value);

                    if (dto is null)
                    {
                        _logger.LogWarning("Mensaje nulo recibido. Offset: {Offset}", result.Offset);
                        continue;
                    }

                    if (_handlers.TryGetValue(dto.Channel, out var handler))
                    {
                        _logger.LogInformation("Procesando evento {Id} por canal {Channel}", dto.Id, dto.Channel);
                        handler(dto);
                    }
                    else
                    {
                        _logger.LogError("Canal no soportado: {Channel}. Evento: {Id}", dto.Channel, dto.Id);
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error al consumir mensaje de Kafka");
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Error al deserializar mensaje Kafka");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error inesperado en NotificationConsumer");
                }
            }
        }, stoppingToken);
    }

    public override void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
        base.Dispose();
    }
}
