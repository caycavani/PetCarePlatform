using Microsoft.EntityFrameworkCore;
using PetCare.Payment.Domain.Interfaces;
using PetCare.Payment.Infrastructure.Persistence;
using PetCare.Payment.Infrastructure.Repositories;
using PetCare.Payment.Application.UseCases;

var builder = WebApplication.CreateBuilder(args);

// 🔧 Servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔌 DbContext
builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🧩 Inyección de dependencias
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<ProcessPaymentUseCase>();

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
