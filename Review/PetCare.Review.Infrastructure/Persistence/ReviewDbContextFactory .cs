namespace PetCare.Review.Infrastructure.Persistence
{
    using System;
    using System.IO;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;
    using PetCare.Review.Infrastructure.Persistence;

    public class ReviewDbContextFactory : IDesignTimeDbContextFactory<ReviewDbContext>
    {
        public ReviewDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())!.FullName, "PetCare.Review.Api"))
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("No se encontró la cadena 'DefaultConnection' en appsettings.json.");

            var options = new DbContextOptionsBuilder<ReviewDbContext>();
            options.UseSqlServer(connectionString);

            return new ReviewDbContext(options.Options);
        }
    }
}
