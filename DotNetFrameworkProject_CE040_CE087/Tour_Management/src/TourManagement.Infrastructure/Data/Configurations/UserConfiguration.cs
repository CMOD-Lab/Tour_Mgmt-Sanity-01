using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for the User entity.
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("UserInfo");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256)
            .HasColumnName("Email");

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("FirstName");

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("LastName");

        builder.Property(u => u.Gender)
            .IsRequired()
            .HasMaxLength(20)
            .HasColumnName("Gender");

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnName("Password");

        builder.Property(u => u.DateOfBirth)
            .HasColumnName("dob");

        builder.Property(u => u.Street)
            .HasMaxLength(200)
            .HasColumnName("Street");

        builder.Property(u => u.City)
            .HasMaxLength(100)
            .HasColumnName("City");

        builder.Property(u => u.State)
            .HasMaxLength(100)
            .HasColumnName("State");

        builder.Property(u => u.IsActive)
            .HasDefaultValue(true);

        builder.Property(u => u.IsAdmin)
            .HasDefaultValue(false);

        builder.Property(u => u.CreatedDate)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasMany(u => u.Bookings)
            .WithOne(b => b.User)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(u => u.Email).IsUnique();
    }
}
