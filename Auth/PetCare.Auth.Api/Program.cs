using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PetCare.Auth.Application.Interfaces;
using PetCare.Auth.Application.Services;
using PetCare.Auth.Domain.Interfaces;
using PetCare.Auth.Infrastructure.Persistence;
using PetCare.Auth.Infrastructure.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

// 🔧 Forzar uso del handler clásico y evitar mapeo automático de claims
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

// 📋 Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// 🔍 Mostrar cadena de conexión en logs
var connString = builder.Configuration.GetConnectionString("AuthDatabase")
                 ?? Environment.GetEnvironmentVariable("ConnectionStrings__AuthDatabase");
Console.WriteLine($"🔧 Cadena de conexión EF: {connString}");

// 🧬 Configuración de EF Core
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(connString));

// 🧩 Registro de interfaces y servicios
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// 🔐 Registro directo del generador real
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

// 🔐 Autenticación JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"] ?? string.Empty);
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        // 🚫 Desactivar metadata discovery
        options.Authority = null;
        options.MetadataAddress = null;
        options.Configuration = null;

        // ✅ Forzar uso del handler clásico
        options.SecurityTokenValidators.Clear();
        options.SecurityTokenValidators.Add(new JwtSecurityTokenHandler());

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// 🌐 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// 📘 Swagger con JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PetCare Auth API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando el esquema Bearer",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ✅ Controladores y autorización
builder.Services.AddControllers();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
});

var app = builder.Build();

// 🚀 Validación y migración con lógica de reintento
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

    const int intentosMaximos = 5;
    const int esperaSegundos = 10;

    for (int intento = 1; intento <= intentosMaximos; intento++)
    {
        try
        {
            var databaseCreator = dbContext.Database.GetService<IRelationalDatabaseCreator>();

            if (!databaseCreator.Exists())
            {
                logger.LogInformation("⚠️ Base de datos no existe. Creándola...");
                dbContext.Database.EnsureCreated();
            }

            logger.LogInformation("🧩 Aplicando migraciones...");
            dbContext.Database.Migrate();
            logger.LogInformation("✅ Migraciones aplicadas correctamente");
            break;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, $"❌ Intento {intento} fallido para conectar/migrar base de datos");

            if (intento == intentosMaximos)
            {
                logger.LogError("🚫 No se logró conectar con la base de datos tras múltiples intentos");
                throw;
            }

            Thread.Sleep(esperaSegundos * 1000);
        }
    }
}

// 🌐 Middlewares
app.UseCors("AllowAll");
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/api/Auth/ping", () => Results.Ok("pong"));
app.MapGet("/api/debug/generate-token", (
    IJwtTokenGenerator generator,
    HttpContext context) =>
{
    var remoteIp = context.Connection.RemoteIpAddress?.ToString();
    if (remoteIp != "127.0.0.1" && remoteIp != "::1")
    {
        return Results.Unauthorized();
    }

    var token = generator.GenerateToken(Guid.NewGuid(), "User");
    return Results.Ok(new
    {
        token,
        issuedAt = DateTime.UtcNow,
        expiresAt = DateTime.UtcNow.AddHours(1)
    });
});

app.Run();
