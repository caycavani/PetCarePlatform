using Microsoft.EntityFrameworkCore;
using PetCare.Payment.Domain.Interfaces;
using PetCare.Payment.Infrastructure.Persistence;
using PetCare.Payment.Infrastructure.Repositories;
using PetCare.Payment.Application.UseCases;
using PetCare.Payment.Api.Controllers; // necesario para inyectar HttpClient directamente en el controlador

var builder = WebApplication.CreateBuilder(args);

// 🔧 Servicios MVC + validación
builder.Services.AddControllers()
    .AddDataAnnotationsLocalization(); // opcional si quieres mensajes de validación localizables

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔌 DbContext SQL Server
builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🧩 Inyección de dependencias
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<ProcessPaymentUseCase>();

// 🌐 HttpClient para comunicación con Pets.Api
builder.Services.AddHttpClient<PaymentController>(client =>
{
    client.BaseAddress = new Uri("http://pets-api"); // debe coincidir con el nombre del servicio en docker-compose
});

var app = builder.Build();

// 🚀 Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
