using Microsoft.EntityFrameworkCore;
using PetCare.Auth.Domain.Entities;

namespace PetCare.Auth.Infrastructure.Persistence
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options)
        {
        }

        // 🗂️ DbSets
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // 🧑‍💼 USER config
            builder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Email)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.HasIndex(u => u.Email)
                      .IsUnique();

                entity.Property(u => u.PasswordHash)
                      .IsRequired();

                entity.Property(u => u.FullName)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(u => u.Phone)
                      .HasMaxLength(20)
                      .IsRequired();

                entity.Property(u => u.Username)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.HasIndex(u => u.Username)
                      .IsUnique();

                entity.Property(u => u.IsActive)
                      .IsRequired();

                entity.Property(u => u.CreatedAt)
                      .IsRequired();

                // 📎 Relación con Role
                entity.HasOne(u => u.Role)
                      .WithMany(r => r.Users)
                      .HasForeignKey(u => u.RoleId)
                      .OnDelete(DeleteBehavior.Restrict);

                // 🔁 Relación con RefreshTokens
                entity.HasMany(u => u.RefreshTokens)
                      .WithOne(rt => rt.User)
                      .HasForeignKey(rt => rt.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // 🏷️ ROLE config
            builder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.Property(r => r.Name)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(r => r.NormalizedName)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.HasIndex(r => r.NormalizedName)
                      .IsUnique();
            });

            // 🔁 REFRESH TOKEN config
            builder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(rt => rt.Id);

                entity.Property(rt => rt.Token)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(rt => rt.ExpiresAt)
                      .IsRequired();
            });
        }
    }
}
