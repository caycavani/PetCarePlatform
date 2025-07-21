using Microsoft.EntityFrameworkCore;

namespace Demo;

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

    public DbSet<object> Dummy { get; set; }
}
