using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using PetCare.Booking.Infrastructure.Persistence;
using System;
using System.IO;

namespace PetCare.Booking.Infrastructure.Persistence
{
    public class BookingDbContextFactory : IDesignTimeDbContextFactory<ReservationDbContext>
    {
        public ReservationDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(
                    Directory.GetParent(Directory.GetCurrentDirectory())!.FullName,
                    "PetCare.Booking.Api"))
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var connectionString = config.GetConnectionString("BookingDatabase");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("No se encontró la cadena 'DefaultConnection' en appsettings.json.");

            var options = new DbContextOptionsBuilder<ReservationDbContext>();
            options.UseSqlServer(connectionString);

            return new ReservationDbContext(options.Options);
        }
    }
}
