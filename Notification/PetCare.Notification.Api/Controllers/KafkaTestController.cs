using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace PetCare.Notification.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KafkaTestController : ControllerBase
    {
        [HttpGet("ping")]
        public async Task<IActionResult> PingKafka()
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "petcare_kafka:9092",
                ClientId = "petcare-notification-ping"
            };

            try
            {
                using var producer = new ProducerBuilder<Null, string>(config).Build();
                var result = await producer.ProduceAsync("notification-events", new Message<Null, string>
                {
                    Value = $"Ping desde Notification API - {DateTime.UtcNow}"
                });

                return Ok(new
                {
                    Status = "✅ Conexión exitosa con Kafka",
                    Topic = result.Topic,
                    Partition = result.Partition,
                    Offset = result.Offset
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "❌ Error de conexión con Kafka",
                    Error = ex.Message
                });
            }
        }
    }
}
