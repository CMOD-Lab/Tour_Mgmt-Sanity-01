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

    public DbSet<Tour> Tours { get; set; } = null!;
    public DbSet<Booking> Bookings { get; set; } = null!;
    public DbSet<UserInfo> UserInfos { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new TourConfiguration());
        modelBuilder.ApplyConfiguration(new BookingConfiguration());
        modelBuilder.ApplyConfiguration(new UserInfoConfiguration());
    }
}
