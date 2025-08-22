namespace PetCare.Notification.Application.DTOs.kafka
{
    public class KafkaSettings
    {
        public string BootstrapServers { get; set; } = default!;
        public string Topic { get; set; } = "notification-events";
        public string GroupId { get; set; } = "notification-consumer-group";
        public string ClientId { get; set; } = "notification-consumer";
        public string SecurityProtocol { get; set; } = "PLAINTEXT";
        public int SessionTimeoutMs { get; set; } = 10000;
        public int MaxPollIntervalMs { get; set; } = 300000;
        public bool EnableAutoCommit { get; set; } = true;
        public string AutoOffsetReset { get; set; } = "Earliest";
    }
}
