using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for UserInfo entity.
/// </summary>
public class UserInfoConfiguration : IEntityTypeConfiguration<UserInfo>
{
    /// <summary>
    /// Configures the UserInfo entity.
    /// </summary>
    public void Configure(EntityTypeBuilder<UserInfo> builder)
    {
        builder.ToTable("UserInfo");

        builder.HasKey(u => u.Email);

        builder.Property(u => u.Email)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.FirstName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.Gender)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(u => u.Password)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(u => u.Dob)
            .IsRequired();

        builder.Property(u => u.Street)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.City)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.State)
            .HasMaxLength(50)
            .IsRequired();

        // Relationship: one user has many bookings
        builder.HasMany(u => u.Bookings)
            .WithOne(b => b.User)
            .HasForeignKey(b => b.UserEmail)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
