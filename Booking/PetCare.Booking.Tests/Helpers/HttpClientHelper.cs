using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using PetCare.Auth.Application.Interfaces;
using PetCare.Booking.Tests.Infrastructure;

namespace PetCare.Booking.Tests.Helpers
{
    public static class HttpClientHelper
    {
        public static async Task<HttpClient> GetAuthenticatedClient(CustomWebApplicationFactory factory, Guid clientId)
        {
            var client = factory.CreateClient();

            using var scope = factory.Services.CreateScope();
            var jwt = scope.ServiceProvider.GetRequiredService<IJwtTokenGenerator>();

            // 🛡️ Generar token con ClientId dinámico
            var token = jwt.GenerateToken(clientId, "CLIENTE");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            return client;
        }
    }
}
