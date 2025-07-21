using Microsoft.EntityFrameworkCore;
using PetCare.Auth.Domain.Entities;

namespace PetCare.Auth.Infrastructure.Persistence
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<Role>().HasKey(r => r.Id);

            modelBuilder.Entity<RefreshToken>().HasKey(rt => rt.Id);
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(rt => rt.Id);

                entity.Property(rt => rt.Token)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(rt => rt.ExpiresAt)
                      .IsRequired();

                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(rt => rt.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

        }
    }
}
