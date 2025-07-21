using Microsoft.EntityFrameworkCore;

namespace PetCare.Payment.Infrastructure.Persistence;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options) { }

    public DbSet<object> Dummy { get; set; }
}
