using Microsoft.EntityFrameworkCore;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data;

/// <summary>
/// Entity Framework Core database context for Tour Management application.
/// </summary>
public class TourManagementDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of TourManagementDbContext.
    /// </summary>
    public TourManagementDbContext(DbContextOptions<TourManagementDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// DbSet for UserInfo entities.
    /// </summary>
    public DbSet<UserInfo> UserInfos { get; set; } = null!;

    /// <summary>
    /// DbSet for Tour entities.
    /// </summary>
    public DbSet<Tour> Tours { get; set; } = null!;

    /// <summary>
    /// DbSet for Booking entities.
    /// </summary>
    public DbSet<Booking> Bookings { get; set; } = null!;

    /// <summary>
    /// Configures the entity model.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TourManagementDbContext).Assembly);
    }
}
