// cr-dotnet-1034: Async GridView Data Binding with RDS via Entity Framework Core
// Replaced synchronous OnGet() with async Task OnGetAsync() using Entity Framework Core
// connected to Amazon RDS, preventing thread pool exhaustion under load.
// - Removed: synchronous Dapper SqlConnection data access
// - Added: async/await Task-based pattern with Entity Framework Core DbContext
// - Added: IAsyncEnumerable / ToListAsync() for non-blocking data retrieval
// - Connection string sourced from environment variable DB_CONNECTION_STRING (RDS endpoint)

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tour_Management.Models;

namespace Tour_Management.Pages
{
    // cr-dotnet-1034: PageModel now uses async Task OnGetAsync() instead of synchronous OnGet()
    // This prevents thread pool exhaustion under cloud load and enables efficient auto-scaling on AWS.
    public class DisplayToursModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DisplayToursModel> _logger;

        public List<Tour> Tours { get; set; } = new List<Tour>();

        public DisplayToursModel(IConfiguration configuration, ILogger<DisplayToursModel> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        // cr-dotnet-1034: Replaced synchronous OnGet() with async Task OnGetAsync()
        // Uses Entity Framework Core ToListAsync() for non-blocking RDS data access
        public async Task OnGetAsync()
        {
            try
            {
                var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                    ?? _configuration.GetConnectionString("dbconnection");

                var optionsBuilder = new DbContextOptionsBuilder<TourDbContext>();
                optionsBuilder.UseSqlServer(connectionString);

                // cr-dotnet-1034: async EF Core query replaces synchronous Dapper connection
                using (var dbContext = new TourDbContext(optionsBuilder.Options))
                {
                    Tours = await dbContext.Tours.ToListAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading tours from Amazon RDS database.");
                Tours = new List<Tour>();
            }
        }
    }

    // cr-dotnet-1034: Entity Framework Core DbContext for async data access to Amazon RDS
    public class TourDbContext : DbContext
    {
        public TourDbContext(DbContextOptions<TourDbContext> options) : base(options) { }

        public DbSet<Tour> Tours { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tour>(entity =>
            {
                entity.ToTable("Tour");
                entity.HasKey(e => e.TourId);
                entity.Property(e => e.TourId).HasColumnName("TOUR_ID");
                entity.Property(e => e.TourName).HasColumnName("TOUR_NAME");
                entity.Property(e => e.Pic).HasColumnName("pic");
                entity.Property(e => e.Price).HasColumnName("PRICE");
                entity.Property(e => e.Days).HasColumnName("DAYS");
                entity.Property(e => e.Locations).HasColumnName("LOCATIONS");
            });
        }
    }
}
