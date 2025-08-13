using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetCare.Auth.Application.Interfaces;
using PetCare.Booking.Api;
using PetCare.Booking.Infrastructure.Persistence;
using PetCare.Tests.Mocks;
using System.Linq;

namespace PetCare.Booking.Tests.Infrastructure;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseTestServer(); // ✅ Usa TestServer interno

        builder.ConfigureServices(services =>
        {
            // 🔄 Remover DbContext existente
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ReservationDbContext>));
            if (descriptor is not null)
                services.Remove(descriptor);

            // 🗃️ Usar SQL Server real para tests
            services.AddDbContext<ReservationDbContext>(options =>
            {
                options.UseSqlServer(
                    "Server=localhost;Database=PetCareBookingDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;");
            });

            // 🔐 JWT simulado para pruebas
            services.AddSingleton<IJwtTokenGenerator, FakeJwtTokenGenerator>();
        });
    }
}
