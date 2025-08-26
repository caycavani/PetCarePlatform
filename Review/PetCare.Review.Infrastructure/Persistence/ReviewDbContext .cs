namespace PetCare.Review.Infrastructure.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using PetCare.Review.Domain.Entities;
    using PetCare.Review.Domain.ValueObjects;

    public class ReviewDbContext : DbContext
    {
        public ReviewDbContext(DbContextOptions<ReviewDbContext> options)
            : base(options) { }

        public DbSet<Review> Reviews => Set<Review>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Review>(entity =>
            {
                entity.ToTable("Reviews");
                entity.HasKey(r => r.Id);

                entity.Property(r => r.ReservationId)
                      .IsRequired();

                entity.Property(r => r.AuthorId)
                      .IsRequired();

                // Mapeo de Value Object Rating
                entity.OwnsOne(r => r.Rating, rating =>
                {
                    rating.Property(r => r.Value)
                          .HasColumnName("Rating")
                          .IsRequired();
                });

                // Mapeo de Value Object Comment
                entity.OwnsOne(r => r.Comment, comment =>
                {
                    comment.Property(c => c.Value)
                           .HasColumnName("Comment")
                           .IsRequired()
                           .HasMaxLength(500);
                });

                entity.Property(r => r.CreatedAt)
                      .IsRequired();
            });
        }
    }
}
