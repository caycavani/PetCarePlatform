using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using PetCare.Pets.Infrastructure.Persistence;
using System;
using System.IO;

namespace PetCare.Pets.Infrastructure.Persistence
{
    public class PetDbContextFactory : IDesignTimeDbContextFactory<PetDbContext>
    {
        public PetDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())!.FullName, "PetCare.Pets.Api"))
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("No se encontró la cadena 'DefaultConnection' en appsettings.json.");

            var options = new DbContextOptionsBuilder<PetDbContext>();
            options.UseSqlServer(connectionString);

            return new PetDbContext(options.Options);
        }
    }
}
