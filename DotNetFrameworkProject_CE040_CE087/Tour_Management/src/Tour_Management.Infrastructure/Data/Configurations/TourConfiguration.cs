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

        builder.HasKey(t => t.Id);

        builder.Property(t => t.TourName)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("TOUR_NAME");

        builder.Property(t => t.Place)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("PLACE");

        builder.Property(t => t.Days)
            .IsRequired()
            .HasColumnName("DAYS");

        builder.Property(t => t.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasColumnName("PRICE");

        builder.Property(t => t.Locations)
            .HasMaxLength(500)
            .HasColumnName("LOCATIONS");

        builder.Property(t => t.TourInfo)
            .HasMaxLength(2000)
            .HasColumnName("TOUR_INFO");

        builder.Property(t => t.Pic)
            .HasMaxLength(500)
            .HasColumnName("pic");

        builder.Property(t => t.CreatedDate)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(t => t.IsActive)
            .HasDefaultValue(true);

        builder.HasMany(t => t.Bookings)
            .WithOne(b => b.Tour)
            .HasForeignKey(b => b.TourId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
