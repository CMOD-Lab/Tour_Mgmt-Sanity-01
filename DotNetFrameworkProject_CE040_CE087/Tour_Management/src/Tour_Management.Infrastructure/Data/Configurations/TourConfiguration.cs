using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tour_Management.Domain.Entities;

namespace Tour_Management.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core entity configuration for Tour.
/// </summary>
public class TourConfiguration : IEntityTypeConfiguration<Tour>
{
    public void Configure(EntityTypeBuilder<Tour> builder)
    {
        builder.ToTable("Tour");

        builder.HasKey(t => t.TourId);

        builder.Property(t => t.TourId)
            .HasColumnName("TOUR_ID")
            .ValueGeneratedOnAdd();

        builder.Property(t => t.TourName)
            .HasColumnName("TOUR_NAME")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Place)
            .HasColumnName("PLACE")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Days)
            .HasColumnName("DAYS")
            .IsRequired();

        builder.Property(t => t.Price)
            .HasColumnName("PRICE")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(t => t.Locations)
            .HasColumnName("LOCATIONS")
            .HasMaxLength(500);

        builder.Property(t => t.TourInfo)
            .HasColumnName("TOUR_INFO")
            .HasMaxLength(1000);

        builder.Property(t => t.Pic)
            .HasColumnName("pic")
            .HasMaxLength(500);

        builder.Property(t => t.IsActive)
            .HasDefaultValue(true);

        builder.Property(t => t.CreatedDate)
            .HasDefaultValueSql("GETUTCDATE()");

        // Relationship
        builder.HasMany(t => t.Bookings)
            .WithOne(b => b.Tour)
            .HasForeignKey(b => b.TourId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
