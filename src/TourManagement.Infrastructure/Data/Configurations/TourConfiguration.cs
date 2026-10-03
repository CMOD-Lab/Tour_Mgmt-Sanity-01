using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for the Tour entity.
/// </summary>
public class TourConfiguration : IEntityTypeConfiguration<Tour>
{
    public void Configure(EntityTypeBuilder<Tour> builder)
    {
        builder.ToTable("Tour");

        builder.HasKey(t => t.TourId);

        builder.Property(t => t.TourId)
            .HasColumnName("TOUR_ID")
            .UseIdentityColumn();

        builder.Property(t => t.TourName)
            .HasColumnName("TOUR_NAME")
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(t => t.Place)
            .HasColumnName("PLACE")
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(t => t.Days)
            .HasColumnName("DAYS")
            .IsRequired();

        builder.Property(t => t.Price)
            .HasColumnName("PRICE")
            .IsRequired()
            .HasColumnType("decimal(6,2)");

        builder.Property(t => t.Locations)
            .HasColumnName("LOCATIONS")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.TourInfo)
            .HasColumnName("TOUR_INFO")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Pic)
            .HasColumnName("pic")
            .HasMaxLength(200);
    }
}
