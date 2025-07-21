using Microsoft.EntityFrameworkCore;
using PetCare.Review.Domain.Interfaces;
using PetCare.Review.Infrastructure.Persistence;
using PetCare.Review.Infrastructure.Repositories;
using PetCare.Review.Application.UseCases;

var builder = WebApplication.CreateBuilder(args);

// 📦 Servicios base
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔌 Configuración DbContext
builder.Services.AddDbContext<ReviewDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🧩 Inyección de dependencias
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<SubmitReviewUseCase>();

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
