using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using PetCare.Notification.Infrastructure.Persistence;
using System;
using System.IO;

namespace PetCare.Notification.Infrastructure.Persistence
{
    public class NotificationDbContextFactory : IDesignTimeDbContextFactory<NotificationDbContext>
    {
        public NotificationDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())!.FullName, "PetCare.Notification.Api"))
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("No se encontró la cadena 'DefaultConnection' en appsettings.json.");

            var options = new DbContextOptionsBuilder<NotificationDbContext>();
            options.UseSqlServer(connectionString);

            return new NotificationDbContext(options.Options);
        }
    }
}
