using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetCare.Auth.Application.Interfaces;
using PetCare.Booking.Api;
using PetCare.Booking.Domain.Entities;
using PetCare.Booking.Infrastructure.Persistence;
using PetCare.Booking.Tests.Infrastructure;
using Xunit;

namespace PetCareApp.IntegrationTests.Features.Reservations;

public class ReservationNoteUpdateTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public ReservationNoteUpdateTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private string GenerateJwt(Guid clientId)
    {
        var tokenGenerator = _factory.Services.GetRequiredService<IJwtTokenGenerator>();
        return tokenGenerator.GenerateToken(clientId, "CLIENTE");
    }

    private async Task<Reservation> SeedReservationAsync(Guid clientId, int statusId = 1, string? note = null)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ReservationDbContext>();

        var reservation = new Reservation(
            Guid.NewGuid(),
            clientId,
            Guid.NewGuid(),            // CaregiverId
            Guid.NewGuid(),            // PetId
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(3),
            note
        );

        reservation.ReservationStatusId = statusId;
        await context.Reservations.AddAsync(reservation);
        await context.SaveChangesAsync();

        return reservation;
    }

    [Fact]
    public async Task UpdateNote_WithValidData_ShouldSucceed()
    {
        var clientId = Guid.NewGuid();
        var jwt = GenerateJwt(clientId);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

        var reservation = await SeedReservationAsync(clientId);
        var content = new StringContent(JsonSerializer.Serialize(new { note = "Nota actualizada" }), Encoding.UTF8, "application/json");

        var response = await _client.PutAsync($"/api/reservations/{reservation.Id}/note", content);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ReservationDbContext>();
        var updated = await db.Reservations.FindAsync(reservation.Id);
        updated!.Note.Should().Be("Nota actualizada");
    }

    [Fact]
    public async Task UpdateNote_CanceledReservation_ShouldReturnBadRequest()
    {
        var clientId = Guid.NewGuid();
        var jwt = GenerateJwt(clientId);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

        var reservation = await SeedReservationAsync(clientId, statusId: 3);
        var content = new StringContent(JsonSerializer.Serialize(new { note = "Intento fallido" }), Encoding.UTF8, "application/json");

        var response = await _client.PutAsync($"/api/reservations/{reservation.Id}/note", content);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateNote_WithoutOwnership_ShouldReturnForbidden()
    {
        var ownerId = Guid.NewGuid();
        var requesterId = Guid.NewGuid();
        var jwt = GenerateJwt(requesterId);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

        var reservation = await SeedReservationAsync(ownerId);
        var content = new StringContent(JsonSerializer.Serialize(new { note = "Sin propiedad" }), Encoding.UTF8, "application/json");

        var response = await _client.PutAsync($"/api/reservations/{reservation.Id}/note", content);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task UpdateNote_InvalidNote_ShouldReturnBadRequest(string nota)
    {
        var clientId = Guid.NewGuid();
        var jwt = GenerateJwt(clientId);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

        var reservation = await SeedReservationAsync(clientId);
        var content = new StringContent(JsonSerializer.Serialize(new { note = nota }), Encoding.UTF8, "application/json");

        var response = await _client.PutAsync($"/api/reservations/{reservation.Id}/note", content);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateNote_NonexistentReservation_ShouldReturnNotFound()
    {
        var clientId = Guid.NewGuid();
        var jwt = GenerateJwt(clientId);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

        var idInexistente = Guid.NewGuid();
        var content = new StringContent(JsonSerializer.Serialize(new { note = "Nota huérfana" }), Encoding.UTF8, "application/json");

        var response = await _client.PutAsync($"/api/reservations/{idInexistente}/note", content);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
