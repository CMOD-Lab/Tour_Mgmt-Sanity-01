using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for the Booking entity.
/// </summary>
public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("booking");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.TourName)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("TOUR_NAME");

        builder.Property(b => b.Place)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("PLACE");

        builder.Property(b => b.Email)
            .IsRequired()
            .HasMaxLength(256)
            .HasColumnName("Email");

        builder.Property(b => b.FirstName)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("FirstName");

        builder.Property(b => b.TourId)
            .HasColumnName("TourId");

        builder.Property(b => b.UserId)
            .HasColumnName("UserId");

        builder.Property(b => b.BookingDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(b => b.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(b => b.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(b => b.CreatedBy)
            .IsRequired()
            .HasMaxLength(256)
            .HasDefaultValue("system");

        builder.Property(b => b.ModifiedBy)
            .HasMaxLength(256);

        builder.HasOne(b => b.Tour)
            .WithMany(t => t.Bookings)
            .HasForeignKey(b => b.TourId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(b => b.User)
            .WithMany(u => u.Bookings)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(b => b.Email);
    }
}
