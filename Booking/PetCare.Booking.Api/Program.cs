using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PetCare.Booking.Application.Interfaces;
using PetCare.Booking.Application.Services;
using PetCare.Booking.Domain.Interfaces;
using PetCare.Booking.Infrastructure.ExternalServices.Pets;
using PetCare.Booking.Infrastructure.ExternalServices.Pets.Interfaces;
using PetCare.Booking.Infrastructure.Persistence;
using PetCare.Booking.Infrastructure.Repositories;
using PetCare.Shared.DTOs.Utils;
using System.Text;

namespace PetCare.Booking.Api
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ✅ Lectura reproducible de configuración
            builder.Configuration
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            var configuration = builder.Configuration;

            // 🔧 Servicios base
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // ✅ Swagger con soporte JWT
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "PetCare.Booking API",
                    Version = "v1"
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Ingresa el token JWT con el prefijo 'Bearer'. Ejemplo: Bearer {token}"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });

            // 🧠 Servicios de dominio
            builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
            builder.Services.AddScoped<IReservationService, ReservationService>();
            builder.Services.AddScoped<IServiceRepository, ServiceRepository>();

            // 🌐 Cliente HTTP para microservicio de mascotas
            builder.Services.AddHttpClient<IPetServiceClient, PetServiceClient>()
                .ConfigureHttpClient(client =>
                {
                    client.BaseAddress = new Uri(configuration["Services:PetApi"]);
                });

            // 🔐 Configuración JWT centralizada
            builder.Services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
            var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>();

            Console.WriteLine("🔍 JWT Settings cargados:");
            Console.WriteLine($"  Issuer:   {jwtSettings.Issuer}");
            Console.WriteLine($"  Audience: {jwtSettings.Audience}");
            Console.WriteLine($"  Secret:   {(string.IsNullOrWhiteSpace(jwtSettings.Secret) ? "[VACÍO]" : "[OK]")}");

            if (string.IsNullOrWhiteSpace(jwtSettings?.Secret) ||
                string.IsNullOrWhiteSpace(jwtSettings.Issuer) ||
                string.IsNullOrWhiteSpace(jwtSettings.Audience))
            {
                throw new InvalidOperationException("❌ Configuración JWT incompleta. Verifica 'Jwt:Secret', 'Jwt:Issuer' y 'Jwt:Audience'.");
            }

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSettings.Secret)
                        ),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            builder.Services.AddAuthorization();

            // 🗃️ Base de datos SQL Server persistente con resiliencia
            builder.Services.AddDbContext<ReservationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions => sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null
                    )
                )
            );

            var app = builder.Build();

            // 🌍 Swagger para desarrollo
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "PetCare.Booking API v1");
                    options.DocumentTitle = "PetCare.Booking Swagger UI";
                });
            }

            // 🔗 Middleware HTTP
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            // ✅ Endpoint de diagnóstico JWT
            app.MapGet("/api/debug/jwt-config", () =>
            {
                var issuer = jwtSettings.Issuer;
                var audience = jwtSettings.Audience;
                var secretLength = string.IsNullOrWhiteSpace(jwtSettings.Secret) ? 0 : jwtSettings.Secret.Length;

                return Results.Ok(new { issuer, audience, secretLength });
            });

            app.Run();
        }
    }
}
