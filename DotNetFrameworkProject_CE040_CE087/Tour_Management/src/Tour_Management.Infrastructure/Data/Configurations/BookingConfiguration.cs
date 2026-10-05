using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tour_Management.Domain.Entities;

namespace Tour_Management.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core entity configuration for Booking.
/// </summary>
public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("booking");

        builder.HasKey(b => b.TourId);

        builder.Property(b => b.TourId)
            .HasColumnName("TOUR_ID")
            .ValueGeneratedOnAdd();

        builder.Property(b => b.TourName)
            .HasColumnName("TOUR_NAME")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.Place)
            .HasColumnName("PLACE")
            .HasMaxLength(200);

        builder.Property(b => b.Email)
            .HasColumnName("Email")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.FirstName)
            .HasColumnName("FirstName")
            .HasMaxLength(100);

        builder.Property(b => b.IsActive)
            .HasDefaultValue(true);

        builder.Property(b => b.CreatedDate)
            .HasDefaultValueSql("GETUTCDATE()");
    }
}
