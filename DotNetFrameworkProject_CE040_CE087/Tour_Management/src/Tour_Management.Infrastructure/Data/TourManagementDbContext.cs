using Microsoft.EntityFrameworkCore;
using Tour_Management.Domain.Entities;
using Tour_Management.Infrastructure.Data.Configurations;

namespace Tour_Management.Infrastructure.Data;

/// <summary>
/// Entity Framework Core database context for Tour Management application.
/// </summary>
public class TourManagementDbContext : DbContext
{
    public TourManagementDbContext(DbContextOptions<TourManagementDbContext> options)
        : base(options)
    {
    }

    /// <summary>Gets or sets the Tours DbSet.</summary>
    public DbSet<Tour> Tours { get; set; } = null!;

    /// <summary>Gets or sets the Users DbSet.</summary>
    public DbSet<UserInfo> Users { get; set; } = null!;

    /// <summary>Gets or sets the Bookings DbSet.</summary>
    public DbSet<Booking> Bookings { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new TourConfiguration());
        modelBuilder.ApplyConfiguration(new UserInfoConfiguration());
        modelBuilder.ApplyConfiguration(new BookingConfiguration());
    }
}
