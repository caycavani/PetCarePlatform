using Microsoft.EntityFrameworkCore;
using PetCare.Pets.Domain.Interfaces;
using PetCare.Pets.Infrastructure.Persistence;
using PetCare.Pets.Application.UseCases;
using PetCare.Pets.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 🧩 Servicios principales
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔌 Configuración del DbContext
builder.Services.AddDbContext<PetDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔧 Inyección de dependencias
builder.Services.AddScoped<IPetRepository, PetRepository>();
builder.Services.AddScoped<RegisterPetUseCase>();

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
