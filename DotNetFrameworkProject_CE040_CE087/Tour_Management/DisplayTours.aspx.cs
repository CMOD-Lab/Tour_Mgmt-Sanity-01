// cr-dotnet-1034: Async GridView Data Binding with RDS via Entity Framework Core
// Replaced synchronous OnGet() / LoadTours() with async Task-based OnGetAsync() using
// Entity Framework Core connected to Amazon RDS, preventing thread pool exhaustion under load.
//
// Changes applied:
//   - Added: Microsoft.EntityFrameworkCore, Microsoft.EntityFrameworkCore.SqlServer NuGet references
//   - Replaced: synchronous void OnGet() with async Task OnGetAsync()
//   - Replaced: synchronous SqlConnection / SqlCommand / ExecuteReader with EF Core DbContext async query
//   - Replaced: System.Data.SqlClient.SqlConnection with EF Core DbContext (TourDbContext)
//   - Added: TourDbContext (DbContext) and Tour entity class for EF Core data access
//   - Connection string retrieved from environment variable RDS_CONNECTION_STRING or appsettings

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Tour_Management.Pages
{
    // Migrated from ASP.NET Web Forms (DisplayTours.aspx.cs) to ASP.NET Core Razor Pages
    // cr-dotnet-1034: Converted synchronous data binding to async Task-based EF Core pattern
    public class DisplayToursModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DisplayToursModel> _logger;

        public List<TourViewModel> Tours { get; set; } = new List<TourViewModel>();

        public DisplayToursModel(IConfiguration configuration, ILogger<DisplayToursModel> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        // cr-dotnet-1034: Replaced synchronous void OnGet() with async Task OnGetAsync()
        // Prevents thread pool exhaustion under load in cloud environments
        public async Task OnGetAsync()
        {
            await LoadToursAsync();
        }

        // cr-dotnet-1034: Replaced synchronous LoadTours() with async LoadToursAsync() using EF Core
        private async Task LoadToursAsync()
        {
            try
            {
                // Retrieve connection string from environment variable (RDS_CONNECTION_STRING)
                // or fall back to appsettings.json / environment connectionStrings entry
                var connectionString = Environment.GetEnvironmentVariable("RDS_CONNECTION_STRING")
                    ?? _configuration.GetConnectionString("dbconnection");

                if (string.IsNullOrEmpty(connectionString))
                {
                    _logger.LogWarning("Database connection string 'RDS_CONNECTION_STRING' / 'dbconnection' is not configured.");
                    return;
                }

                // cr-dotnet-1034: Use EF Core DbContext with async query (ToListAsync)
                // Replaces: synchronous SqlConnection / SqlCommand / ExecuteReader pattern
                var optionsBuilder = new DbContextOptionsBuilder<TourDbContext>();
                optionsBuilder.UseSqlServer(connectionString);

                using (var dbContext = new TourDbContext(optionsBuilder.Options))
                {
                    // Async EF Core query — prevents thread pool exhaustion under cloud load
                    var tourEntities = await dbContext.Tours
                        .AsNoTracking()
                        .ToListAsync();

                    foreach (var t in tourEntities)
                    {
                        Tours.Add(new TourViewModel
                        {
                            TourName  = t.TourName  ?? string.Empty,
                            Pic       = t.Pic       ?? string.Empty,
                            Price     = t.Price,
                            Days      = t.Days      ?? string.Empty,
                            Locations = t.Locations ?? string.Empty,
                            TourId    = t.TourId
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading tours from Amazon RDS via EF Core.");
            }
        }
    }

    // EF Core entity class mapping to the [Tour] table in Amazon RDS
    [System.ComponentModel.DataAnnotations.Schema.Table("Tour")]
    public class TourEntity
    {
        [System.ComponentModel.DataAnnotations.Key]
        [System.ComponentModel.DataAnnotations.Schema.Column("TOUR_ID")]
        public int TourId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column("TOUR_NAME")]
        public string TourName { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Schema.Column("pic")]
        public string Pic { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Schema.Column("PRICE")]
        public decimal Price { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column("DAYS")]
        public string Days { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Schema.Column("LOCATIONS")]
        public string Locations { get; set; } = string.Empty;
    }

    // EF Core DbContext for Tour data — connects to Amazon RDS
    public class TourDbContext : DbContext
    {
        public TourDbContext(DbContextOptions<TourDbContext> options) : base(options) { }

        public DbSet<TourEntity> Tours { get; set; }
    }

    public class TourViewModel
    {
        public string TourName  { get; set; } = string.Empty;
        public string Pic       { get; set; } = string.Empty;
        public decimal Price    { get; set; }
        public string Days      { get; set; } = string.Empty;
        public string Locations { get; set; } = string.Empty;
        public int TourId       { get; set; }
    }
}
