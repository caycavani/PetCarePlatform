using Microsoft.Extensions.DependencyInjection;
using PetCare.Auth.Application.Interfaces;
using PetCare.Booking.Domain.Entities;
using PetCare.Booking.Tests.Builders;
using PetCare.Booking.Tests.Infrastructure;
using PetCare.Booking.Tests.Seeding;
using System.Net.Http.Headers;

namespace PetCare.Booking.Tests.Helpers
{
    public static class ReservationTestSetupHelper
    {
        public static HttpClient CreateClientConReservaYToken(
            CustomWebApplicationFactory factory,
            out Guid reservaId,
            out Guid clientId)
        {
            // 🏗️ Crear reserva dinámica
            var reservation = new ReservationBuilder()
                .WithStatus(2) // STATUS_ACCEPTED
                .WithNote("Reserva para test helper")
                .Build();

            // 🌱 Sembrar reserva en base de datos
            ReservationSeeder.SeedAsync(factory.Services, reservation).GetAwaiter().GetResult();

            reservaId = reservation.Id;
            clientId = reservation.ClientId;

            // 🔐 Obtener token dinámico desde servicio real
            using var scope = factory.Services.CreateScope();
            var tokenService = scope.ServiceProvider.GetRequiredService<IJwtTokenGenerator>();
            var token = tokenService.GenerateToken(clientId, "Client");

            // 🌐 Crear cliente HTTP autenticado
            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return client;
        }
    }
}
