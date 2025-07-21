using Microsoft.EntityFrameworkCore;
using PetCare.Pets.Domain.Entities;

namespace PetCare.Pets.Infrastructure.Persistence
{
    public class PetDbContext : DbContext
    {
        public PetDbContext(DbContextOptions<PetDbContext> options)
            : base(options) { }

        public DbSet<Pet> Pets => Set<Pet>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Pet>(entity =>
            {
                entity.ToTable("Pets");
                entity.HasKey(p => p.Id);

                entity.Property(p => p.OwnerId)
                      .IsRequired();

                entity.Property(p => p.Name)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(p => p.Type)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(p => p.BirthDate)
                      .IsRequired();
            });
        }
    }
}
