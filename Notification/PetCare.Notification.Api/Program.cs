using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PetCare.Notification.Application.Interfaces;
using PetCare.Notification.Application.Kafka;
using PetCare.Notification.Domain.Interfaces;
using PetCare.Notification.Infrastructure.kafka;
using PetCare.Notification.Infrastructure.Kafka;
using PetCare.Notification.Infrastructure.Persistence;
using PetCare.Notification.Infrastructure.Repositories;
using PetCare.Shared.DTOs.Utils; // ✅ Importación de JwtSettings
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// 🔧 Configuración de servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 📘 Swagger con JWT
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PetCare.Notification API",
        Version = "v1",
        Description = "Microservicio para publicación de eventos de notificación vía Kafka"
    });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Token JWT para autenticación",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

// 🔌 Configuración de DbContext
builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

// 🧩 Inyección de dependencias
builder.Services.AddSingleton<INotificationProducer, NotificationProducer>();

// 📦 Configuración de Kafka desde appsettings.json
builder.Services.Configure<KafkaSettings>(configuration.GetSection("Kafka"));
builder.Services.AddHostedService<NotificationConsumer>();

// 🔐 Configuración JWT centralizada
builder.Services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>();

if (string.IsNullOrWhiteSpace(jwtSettings?.Secret))
    throw new InvalidOperationException("La clave JWT no está configurada correctamente. Verifica 'Jwt:Secret' en appsettings.json.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// 🚀 Middleware
app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PetCare.Notification API v1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
