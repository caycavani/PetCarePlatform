namespace PetCare.Review.Infrastructure.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using PetCare.Review.Domain.Entities;

    public class ReviewDbContext : DbContext
    {
        public ReviewDbContext(DbContextOptions<ReviewDbContext> options)
            : base(options) { }

        public DbSet<Rview> Reviews => Set<Rview>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Rview>(entity =>
            {
                entity.ToTable("Reviews");
                entity.HasKey(r => r.Id);

                entity.Property(r => r.ReservationId)
                      .IsRequired();

                entity.Property(r => r.AuthorId)
                      .IsRequired();

                entity.Property(r => r.Rating)
                      .IsRequired();

                entity.Property(r => r.Comment)
                      .IsRequired()
                      .HasMaxLength(1000);

                entity.Property(r => r.CreatedAt)
                      .IsRequired();
            });
        }
    }
}
