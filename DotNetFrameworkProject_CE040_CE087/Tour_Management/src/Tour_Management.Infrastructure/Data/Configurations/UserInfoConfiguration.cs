using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tour_Management.Domain.Entities;

namespace Tour_Management.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core entity configuration for UserInfo.
/// </summary>
public class UserInfoConfiguration : IEntityTypeConfiguration<UserInfo>
{
    public void Configure(EntityTypeBuilder<UserInfo> builder)
    {
        builder.ToTable("UserInfo");

        builder.HasKey(u => u.Email);

        builder.Property(u => u.Email)
            .HasColumnName("Email")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.FirstName)
            .HasColumnName("FirstName")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .HasColumnName("LastName")
            .HasMaxLength(100);

        builder.Property(u => u.Gender)
            .HasColumnName("Gender")
            .HasMaxLength(20);

        builder.Property(u => u.Password)
            .HasColumnName("Password")
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.DateOfBirth)
            .HasColumnName("dob");

        builder.Property(u => u.Street)
            .HasColumnName("Street")
            .HasMaxLength(200);

        builder.Property(u => u.City)
            .HasColumnName("City")
            .HasMaxLength(100);

        builder.Property(u => u.State)
            .HasColumnName("State")
            .HasMaxLength(100);

        builder.Property(u => u.IsActive)
            .HasDefaultValue(true);

        builder.Property(u => u.CreatedDate)
            .HasDefaultValueSql("GETUTCDATE()");
    }
}
