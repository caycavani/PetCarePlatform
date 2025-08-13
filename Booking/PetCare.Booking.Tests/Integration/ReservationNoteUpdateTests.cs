using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PetCare.Auth.Application.Interfaces;
using PetCare.Booking.Api;
using PetCare.Booking.Domain.Entities;
using PetCare.Booking.Domain.Interfaces;
using PetCare.Booking.Tests.Builders;
using PetCare.Booking.Tests.Infrastructure;
using PetCare.Booking.Tests.Seeding;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace PetCare.Booking.Tests.Integration
{
    public class ReservationNoteUpdateTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly Reservation _reservation;
        private readonly IServiceProvider _serviceProvider;

        public ReservationNoteUpdateTests(CustomWebApplicationFactory factory)
        {
            _serviceProvider = factory.Services;

            // 🏗️ Crear reserva válida con estado ACCEPTED
            _reservation = new ReservationBuilder()
                .WithStatus(2) // STATUS_ACCEPTED
                .WithNote("Nota inicial")
                .Build();

            ReservationSeeder.SeedAsync(_serviceProvider, _reservation).GetAwaiter().GetResult();

            // ✅ Verificar persistencia
            using var verifyScope = _serviceProvider.CreateScope();
            var repo = verifyScope.ServiceProvider.GetRequiredService<IReservationRepository>();
            var confirm = repo.GetRawByIdAsync(_reservation.Id).GetAwaiter().GetResult();
            Assert.NotNull(confirm);

            // 🔐 Token del cliente propietario
            var tokenService = verifyScope.ServiceProvider.GetRequiredService<IJwtTokenGenerator>();
            var token = tokenService.GenerateToken(_reservation.ClientId, "CLIENTE");

            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost")
            });
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        [Fact]
        public async Task ShouldUpdateNote_WhenReservationExists()
        {
            var nuevaNota = "Actualización desde test";
            var payload = new { note = nuevaNota };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"/api/reservations/{_reservation.Id}/note", content);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Validar que la nota fue actualizada
            using var scope = _serviceProvider.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IReservationRepository>();
            var actualizada = await repo.GetRawByIdAsync(_reservation.Id);

            Assert.NotNull(actualizada);
            Assert.Equal(nuevaNota, actualizada!.Note);
        }

        [Fact]
        public async Task ShouldRejectNoteUpdate_WhenClientIsNotOwner()
        {
            var tokenService = _serviceProvider.GetRequiredService<IJwtTokenGenerator>();
            var otroToken = tokenService.GenerateToken(Guid.NewGuid(), "CLIENTE");

            var clientNoAutorizado = new HttpClient();
            clientNoAutorizado.BaseAddress = new Uri("http://localhost");
            clientNoAutorizado.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", otroToken);

            var payload = new { note = "Intento no autorizado" };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await clientNoAutorizado.PutAsync($"/api/reservations/{_reservation.Id}/note", content);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task ShouldFailToUpdateNote_WhenReservationIsRejectedOrCancelled()
        {
            var reservaInvalida = new ReservationBuilder()
                .WithStatus(4) // 4 = STATUS_REJECTED, usa 5 para STATUS_CANCELLED si aplica
                .WithNote("Nota previa al rechazo")
                .Build();

            await ReservationSeeder.SeedAsync(_serviceProvider, reservaInvalida);

            var tokenService = _serviceProvider.GetRequiredService<IJwtTokenGenerator>();
            var token = tokenService.GenerateToken(reservaInvalida.ClientId, "CLIENTE");

            var http = new HttpClient();
            http.BaseAddress = new Uri("http://localhost");
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var payload = new { note = "Intento de actualización no permitida" };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await http.PutAsync($"/api/reservations/{reservaInvalida.Id}/note", content);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
