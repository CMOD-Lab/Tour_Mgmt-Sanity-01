// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages (cr-dotnet-0026)
// cr-dotnet-1034: Replaced synchronous GridView/SqlDataSource data binding with
//   async Task-based patterns using Entity Framework Core connected to Amazon RDS.
// Removed: System.Web, System.Web.UI, System.Web.UI.WebControls (Web Forms dependencies)
// Added: Microsoft.AspNetCore.Mvc.RazorPages, Microsoft.EntityFrameworkCore (ASP.NET Core / EF Core)
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Tour_Management.Pages
{
    // cr-dotnet-1034: Migrated from synchronous System.Web.UI.Page to async PageModel
    // using Entity Framework Core to prevent thread pool exhaustion under cloud load.
    public class DisplayToursModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DisplayToursModel> _logger;

        public List<TourItem> Tours { get; set; } = new List<TourItem>();

        public DisplayToursModel(IConfiguration configuration, ILogger<DisplayToursModel> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves the database connection string from environment variable (AWS RDS endpoint)
        /// with fallback to appsettings.json / Web.config for local development.
        /// </summary>
        private string GetConnectionString()
        {
            string envConnStr = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            if (!string.IsNullOrEmpty(envConnStr))
                return envConnStr;
            return _configuration.GetConnectionString("dbconnection");
        }

        /// <summary>
        /// cr-dotnet-1034: Async OnGetAsync replaces synchronous Page_Load / GridView.DataBind().
        /// Uses Entity Framework Core async query (ToListAsync) connected to Amazon RDS,
        /// preventing thread pool exhaustion and enabling efficient auto-scaling.
        /// Replaces: asp:GridView DataSourceID="SqlDataSource1" (synchronous binding)
        ///           asp:SqlDataSource SelectCommand="SELECT [TOUR_NAME], [pic], [PRICE], [DAYS], [LOCATIONS], [TOUR_ID] FROM [Tour]"
        /// </summary>
        public async Task OnGetAsync()
        {
            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<TourDbContext>();
                optionsBuilder.UseSqlServer(GetConnectionString());

                using (var dbContext = new TourDbContext(optionsBuilder.Options))
                {
                    // Async EF Core query — replaces synchronous GridView data binding
                    Tours = await dbContext.Tours
                        .AsNoTracking()
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading tours from database.");
                Tours = new List<TourItem>();
            }
        }
    }

    /// <summary>
    /// Entity model representing a Tour record.
    /// cr-dotnet-1034: Replaces auto-generated columns from asp:GridView DataKeyNames="TOUR_ID".
    /// </summary>
    [System.ComponentModel.DataAnnotations.Schema.Table("Tour")]
    public class TourItem
    {
        [System.ComponentModel.DataAnnotations.Key]
        [System.ComponentModel.DataAnnotations.Schema.Column("TOUR_ID")]
        public int TourId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column("TOUR_NAME")]
        public string TourName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column("pic")]
        public string Pic { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column("PRICE")]
        public decimal Price { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column("DAYS")]
        public string Days { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column("LOCATIONS")]
        public string Locations { get; set; }
    }

    /// <summary>
    /// EF Core DbContext for Tour data — replaces SqlDataSource control.
    /// cr-dotnet-1034: Enables async data access patterns via Entity Framework Core
    /// connected to Amazon RDS, preventing thread pool exhaustion under cloud load.
    /// </summary>
    public class TourDbContext : DbContext
    {
        public TourDbContext(DbContextOptions<TourDbContext> options) : base(options) { }

        public DbSet<TourItem> Tours { get; set; }
    }
}
