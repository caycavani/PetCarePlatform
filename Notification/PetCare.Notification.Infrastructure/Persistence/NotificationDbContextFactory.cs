using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace PetCare.Notification.Infrastructure.Persistence
{
    /// <summary>
    /// Fábrica de contexto utilizada en tiempo de diseño para ejecutar migraciones.
    /// </summary>
    public class NotificationDbContextFactory : IDesignTimeDbContextFactory<NotificationDbContext>
    {
        public NotificationDbContext CreateDbContext(string[] args)
        {
            // Ruta base del proyecto API (ajustable según estructura de solución)
            var basePath = Path.Combine(
                Directory.GetParent(Directory.GetCurrentDirectory())!.FullName,
                "PetCare.Notification.Api"
            );

            // Carga de configuración desde appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Obtención de cadena de conexión
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "No se encontró la cadena de conexión 'DefaultConnection' en appsettings.json."
                );
            }

            // Configuración del DbContext con SQL Server
            var optionsBuilder = new DbContextOptionsBuilder<NotificationDbContext>();
            optionsBuilder.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(NotificationDbContextFactory).Assembly.FullName);
                sqlOptions.EnableRetryOnFailure(3); // Reintentos ante fallos transitorios
            });

            return new NotificationDbContext(optionsBuilder.Options);
        }
    }
}
