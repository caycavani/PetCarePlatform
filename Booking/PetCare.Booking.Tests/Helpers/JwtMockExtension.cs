using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using PetCare.Auth.Application.Interfaces;
using PetCare.Booking.Api;

namespace PetCare.Booking.Tests.Helpers
{
    public static class JwtMockExtension
    {
        public static WebApplicationFactory<Program> WithMockJwt(this WebApplicationFactory<Program> factory)
        {
            return factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var jwtMock = new Mock<IJwtTokenGenerator>();
                    jwtMock.Setup(j => j.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>()))
                           .Returns("mocked-jwt-token");

                    services.AddSingleton(jwtMock.Object);
                });
            });
        }
    }
}
