using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using PetCare.Auth.Infrastructure.Persistence;
using System.IO;

namespace PetCare.Auth.Infrastructure.Persistence
{
    public class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
    {
        public AuthDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "PetCare.Auth.Api"))
                   .AddJsonFile("appsettings.json", optional: false)
                   .Build();

            var options = new DbContextOptionsBuilder<AuthDbContext>();
            options.UseSqlServer(config.GetConnectionString("AuthDatabase"));

            return new AuthDbContext(options.Options);
        }
    }
}
