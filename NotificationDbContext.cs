using Microsoft.EntityFrameworkCore;

namespace PetCare.Notification.Infrastructure.Persistence;

public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options) { }

    public DbSet<object> Dummy { get; set; }
}
