namespace PetCare.Review.Infrastructure.Persistence
{
    using System;
    using System.IO;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;

    public class ReviewDbContextFactory : IDesignTimeDbContextFactory<ReviewDbContext>
    {
        public ReviewDbContext CreateDbContext(string[] args)
        {
            var projectRoot = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName
                              ?? throw new InvalidOperationException("No se pudo determinar el directorio raíz del proyecto.");

            var configPath = Path.Combine(projectRoot, "PetCare.Review.Api");

            var config = new ConfigurationBuilder()
                .SetBasePath(configPath)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("No se encontró la cadena 'DefaultConnection' en appsettings.json.");

            var optionsBuilder = new DbContextOptionsBuilder<ReviewDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new ReviewDbContext(optionsBuilder.Options);
        }
    }
}
