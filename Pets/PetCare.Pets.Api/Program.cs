using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PetCare.Pets.Application.Interfaces;
using PetCare.Pets.Application.Services;
using PetCare.Pets.Domain.Interfaces;
using PetCare.Pets.Infrastructure.Persistence;
using PetCare.Pets.Infrastructure.Repositories;
using PetCare.Shared.DTOs.Utils;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// 🔧 Servicios base
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 📘 Swagger con JWT
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PetCare.Pets API",
        Version = "v1",
        Description = "Microservicio para gestión de mascotas"
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
builder.Services.AddDbContext<PetDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("PetsDatabase")));

// 🧩 Inyección de dependencias
builder.Services.AddScoped<IPetService, PetService>();
builder.Services.AddScoped<IPetRepository, PetRepository>();

// 🔐 Configuración JWT
builder.Services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>();

if (string.IsNullOrWhiteSpace(jwtSettings?.Secret))
{
    Console.WriteLine("❌ Error: Jwt:Secret no está configurado.");
    throw new InvalidOperationException("La clave JWT no está configurada correctamente. Verifica 'Jwt:Secret' en appsettings.json.");
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
        };
    });

builder.Services.AddAuthorization();

// 🛠️ Forzar ASP.NET a escuchar en el puerto 80 (alineado con docker-compose)
//builder.WebHost.UseUrls("http://*:80");
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80); // 🔥 Esto fuerza el uso del puerto 80 dentro del contenedor
});

var app = builder.Build();

// 🚀 Middleware
app.UseSwagger(); // 🔥 Activado sin condicional para entorno contenedor
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PetCare.Pets API v1");
    c.RoutePrefix = "swagger"; // Asegura que /swagger/index.html funcione
});

// ❌ HTTPS deshabilitado para entorno local en contenedor
// app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// 🧪 Diagnóstico de entorno
Console.WriteLine("✅ Pets API iniciado en entorno: " + app.Environment.EnvironmentName);
app.Run();
