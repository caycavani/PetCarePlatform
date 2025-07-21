using Microsoft.EntityFrameworkCore;
using PetCare.Notification.Domain.Interfaces;
using PetCare.Notification.Infrastructure.Persistence;
using PetCare.Notification.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 🔧 Configuración de servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔌 Configuración de DbContext
builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🧩 Inyección de dependencias
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

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
