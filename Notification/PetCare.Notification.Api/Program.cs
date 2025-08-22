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
using System.Text;

var builder = WebApplication.CreateBuilder(args);

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
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🧩 Inyección de dependencias
//builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddSingleton<INotificationProducer, NotificationProducer>();

// 📦 Configuración de Kafka desde appsettings.json
builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("Kafka"));
// 📦 Consumer de Kafka
builder.Services.AddHostedService<NotificationConsumer>();


// 🔐 Configuración de autenticación JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings.GetValue<string>("Secret");

if (string.IsNullOrWhiteSpace(secretKey))
    throw new InvalidOperationException("La clave JWT no está configurada correctamente. Verifica 'Jwt:Secret' en appsettings.json.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.GetValue<string>("Issuer"),
            ValidAudience = jwtSettings.GetValue<string>("Audience"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

var app = builder.Build();

// 🚀 Middleware
app.UseRouting(); // 🔄 Asegura el orden correcto

// 🧪 Swagger disponible en todos los entornos (puedes limitarlo si prefieres)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PetCare.Notification API v1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseAuthentication(); // 🔐 JWT
app.UseAuthorization();

app.MapControllers();

app.Run();
