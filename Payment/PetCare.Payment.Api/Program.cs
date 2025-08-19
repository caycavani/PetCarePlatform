using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PetCare.Payment.Application.Services;
using PetCare.Payment.Domain.Interfaces;
using PetCare.Payment.Infrastructure.Gateways;
using PetCare.Payment.Infrastructure.Gateways.Mock;
using PetCare.Payment.Infrastructure.Gateways.PSE;
using PetCare.Payment.Infrastructure.Gateways.Wompi;
using PetCare.Payment.Infrastructure.Persistence;
using PetCare.Payment.Infrastructure.Repositories;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 🔧 Servicios MVC + validación
builder.Services.AddControllers()
    .AddDataAnnotationsLocalization();

builder.Services.AddEndpointsApiExplorer();

// 📦 Swagger con autenticación JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PetCare.Payment", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa el token JWT con el prefijo 'Bearer '"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// 🔐 Configuración de autenticación JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Jwt:Authority"];
        options.RequireHttpsMetadata = builder.Configuration.GetValue<bool>("Jwt:RequireHttps");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"])
            )
        };
    });

// 🔌 DbContext SQL Server
builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🧪 HealthChecks (✅ Corregido)
/*builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));*/

// 🌐 CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>();
        policy.WithOrigins(origins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 🧩 Inyección de dependencias
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddTransient<StripeGatewayClient>();
builder.Services.AddTransient<WompiGatewayClient>();
builder.Services.AddTransient<PseGatewayClient>();
builder.Services.AddTransient<MockPaymentGatewayClient>();
builder.Services.AddSingleton<PaymentGatewaySelector>();
builder.Services.AddScoped<PaymentService>();

var app = builder.Build();

// 🚀 Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
//app.MapHealthChecks("/health");

app.Run();
