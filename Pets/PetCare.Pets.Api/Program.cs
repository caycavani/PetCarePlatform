using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PetCare.Pets.Application.Interfaces;
using PetCare.Pets.Application.Services;
using PetCare.Pets.Domain.Interfaces;
using PetCare.Pets.Infrastructure.Persistence;
using PetCare.Pets.Infrastructure.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

// 🔧 Evitar mapeo automático de claims
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

// ✅ Configuración
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// 🧩 Servicios base
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 📦 Swagger con soporte JWT
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Pets API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa el token JWT con el prefijo Bearer. Ejemplo: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// 🔐 Autenticación JWT sin metadata discovery
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"] ?? string.Empty);
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        // ✅ Forzar uso del handler clásico
        options.UseSecurityTokenValidators = true;
        options.SecurityTokenValidators.Clear();
        options.SecurityTokenValidators.Add(new JwtSecurityTokenHandler());

        // 🚫 Desactivar metadata discovery
        options.Authority = null;
        options.MetadataAddress = null;
        options.Configuration = null;

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

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");
                if (!string.IsNullOrEmpty(token))
                {
                    Console.WriteLine($"🔍 Token recibido: {token}");
                }
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"❌ Error de autenticación: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("✅ Token validado correctamente");
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Console.WriteLine($"⚠️ Challenge: {context.Error}, {context.ErrorDescription}");
                return Task.CompletedTask;
            }
        };
    });

// ✅ Política permisiva como predeterminada
builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAssertion(_ => true)
        .Build();

    options.FallbackPolicy = options.DefaultPolicy;
});

// 🗃️ DbContext con SQL Server
builder.Services.AddDbContext<PetDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("PetsDatabase"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
        )
    )
);

// 🔧 Inyección de dependencias
builder.Services.AddScoped<IPetService, PetService>();
builder.Services.AddScoped<IPetRepository, PetRepository>();

// 🔊 Escuchar en el puerto 80 dentro del contenedor
builder.WebHost.UseUrls("http://*:80");

var app = builder.Build();

// 🚀 Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Pets API v1");
        options.DocumentTitle = "Pets Swagger UI";
    });
}

app.UseHttpsRedirection();

// 🧪 Middleware de diagnóstico
app.Use(async (context, next) =>
{
    var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
    Console.WriteLine($"🔍 Authorization Header recibido: {authHeader}");
    await next();
});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

// 🧪 Diagnóstico final
var validatorLog = app.Services.GetRequiredService<IOptions<JwtBearerOptions>>().Value.SecurityTokenValidators;
Console.WriteLine("🔍 Validadores JWT activos:");
foreach (var validator in validatorLog)
{
    Console.WriteLine($"🔧 Validator: {validator.GetType().Name}");
}
