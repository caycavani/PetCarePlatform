namespace PetCare.Notification.Application.Kafka
{
    /// <summary>
    /// Representa la configuración de Kafka cargada desde appsettings.json.
    /// </summary>
    public class KafkaSettings
    {
        /// <summary>
        /// Dirección del servidor de Kafka (ej. localhost:9092).
        /// </summary>
        public string BootstrapServers { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del topic donde se publican los eventos de notificación.
        /// </summary>
        public string Topic { get; set; } = string.Empty;

        /// <summary>
        /// Identificador del productor (opcional, útil para trazabilidad).
        /// </summary>
        public string ClientId { get; set; } = "notification-producer";

        /// <summary>
        /// Número de reintentos en caso de fallo al enviar el mensaje.
        /// </summary>
        public int RetryCount { get; set; } = 3;

        /// <summary>
        /// Tiempo máximo de espera para enviar un mensaje (en milisegundos).
        /// </summary>
        public int MessageTimeoutMs { get; set; } = 5000;

        /// <summary>
        /// Activar logs detallados para debugging.
        /// </summary>
        public bool EnableDebugLogging { get; set; } = false;
    }
}

