using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PetCare.Review.Application.Interfaces;
using PetCare.Review.Application.Services;
using PetCare.Review.Domain.Interfaces;
using PetCare.Review.Domain.Interfaces.kafka.Eventos;
using PetCare.Review.Domain.Interfaces.kafka.Validation;
using PetCare.Review.Infrastructure.Persistence;
using PetCare.Review.Infrastructure.Publishers;
using PetCare.Review.Infrastructure.Repositories;
using PetCare.Review.Infrastructure.Validators;
using PetCare.Shared.DTOs.Utils;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 🔧 Configuración reproducible
builder.Configuration.Sources.Clear();
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

var configuration = builder.Configuration;

// 🔧 Servicios base
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 🌐 CORS global
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 📘 Swagger con JWT
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PetCare Review API",
        Version = "v1",
        Description = "API para gestión de reseñas en PetCare Platform"
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

// 🔌 DbContext
builder.Services.AddDbContext<ReviewDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

// 🧩 Inyección de dependencias
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IReviewValidator, ReviewValidator>();
builder.Services.AddScoped<IReviewPublisher, ReviewPublisher>();

// 🔐 JWT Authentication con JwtSettings
builder.Services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
var jwtConfig = configuration.GetSection("Jwt").Get<JwtSettings>();

Console.WriteLine("🔍 JWT Settings cargados:");
Console.WriteLine($"  Issuer:   {jwtConfig.Issuer}");
Console.WriteLine($"  Audience: {jwtConfig.Audience}");
Console.WriteLine($"  Secret:   {(string.IsNullOrWhiteSpace(jwtConfig.Secret) ? "[VACÍO]" : "[OK]")}");

if (string.IsNullOrWhiteSpace(jwtConfig?.Secret) ||
    string.IsNullOrWhiteSpace(jwtConfig.Issuer) ||
    string.IsNullOrWhiteSpace(jwtConfig.Audience))
{
    throw new InvalidOperationException("❌ Configuración JWT incompleta. Verifica 'Jwt:Secret', 'Jwt:Issuer' y 'Jwt:Audience'.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var path = context.HttpContext.Request.Path;
                if (path.StartsWithSegments("/swagger") || path.StartsWithSegments("/api/debug/jwt-config"))
                {
                    context.NoResult();
                    return Task.CompletedTask;
                }

                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"[JWT ERROR] {context.Exception.Message}");
                return Task.CompletedTask;
            }
        };

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtConfig.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtConfig.Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// 🚀 Middleware
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PetCare Review API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ✅ Endpoint de diagnóstico JWT
app.MapGet("/api/debug/jwt-config", () =>
{
    var issuer = jwtConfig.Issuer;
    var audience = jwtConfig.Audience;
    var secretLength = string.IsNullOrWhiteSpace(jwtConfig.Secret) ? 0 : jwtConfig.Secret.Length;

    return Results.Ok(new { issuer, audience, secretLength });
});

app.Run();
