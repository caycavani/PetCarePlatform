using System.Net;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using PetCare.Booking.Domain.Entities;
using PetCare.Booking.Tests.Builders;
using PetCare.Booking.Tests.Helpers;
using PetCare.Booking.Tests.Infrastructure;
using PetCare.Booking.Tests.Seeding;
using PetCare.Booking.Domain.Interfaces;
using Xunit;

namespace PetCare.Booking.Tests.Controllers
{
    public class ReservationControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly Reservation _reservation;

        public ReservationControllerTests(CustomWebApplicationFactory factory)
        {
            // 🔐 Generar un ClientId dinámico para autenticación
            var clientId = Guid.NewGuid();

            // 🏗️ Construir la reserva vinculada al ClientId
            _reservation = new ReservationBuilder()
                .WithClientId(clientId)
                .WithNote("Reserva controlada para prueba")
                .WithStatus(2) // STATUS_ACCEPTED
                .Build();

            // 🌱 Sembrar la reserva directamente en la base
            ReservationSeeder.SeedAsync(factory.Services, _reservation).GetAwaiter().GetResult();

            // ✅ Verificar que esté presente en la base
            using var scope = factory.Services.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IReservationRepository>();
            var confirm = repo.GetRawByIdAsync(_reservation.Id).GetAwaiter().GetResult();
            Assert.NotNull(confirm);

            // 🔐 Crear cliente autenticado con el mismo ClientId de la reserva
            _client = HttpClientHelper.GetAuthenticatedClient(factory, _reservation.ClientId).Result;
        }

        [Fact]
        public async Task GetReservationById_ReturnsSuccess()
        {
            // 🧪 Mostrar ID y ClientId para rastreo
            Console.WriteLine($"🔎 Solicitando reserva con ID: {_reservation.Id}");
            Console.WriteLine($"🔐 Autenticado como ClientId: {_reservation.ClientId}");

            // 🛰️ Consultar reserva por ID
            var response = await _client.GetAsync($"/api/reservations/{_reservation.Id}");

            // 📡 Mostrar status HTTP recibido
            Console.WriteLine($"🌐 Código HTTP recibido: {response.StatusCode}");

            // ✅ Verificar respuesta HTTP 200 OK
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // 📦 Leer y mostrar JSON
            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine("📦 JSON recibido:");
            Console.WriteLine(json);

            // 🧬 Deserializar JSON al DTO
            var data = JsonSerializer.Deserialize<ReservationResponse>(json, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // 🛡️ Validar objeto deserializado
            Assert.NotNull(data);
            Assert.Equal(_reservation.Id, data!.Id);
            Console.WriteLine($"✅ Reserva recibida: {data.Note}");
        }
    }
}
