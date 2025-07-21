using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using PetCare.Payment.Infrastructure.Persistence;
using System;
using System.IO;

namespace PetCare.Payment.Infrastructure.Persistence
{
    public class PaymentDbContextFactory : IDesignTimeDbContextFactory<PaymentDbContext>
    {
        public PaymentDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())!.FullName, "PetCare.Payment.Api"))
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("No se encontró la cadena 'DefaultConnection' en appsettings.json.");

            var options = new DbContextOptionsBuilder<PaymentDbContext>();
            options.UseSqlServer(connectionString);

            return new PaymentDbContext(options.Options);
        }
    }
}
