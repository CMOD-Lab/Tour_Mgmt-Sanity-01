using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Tour_Management.Pages
{
    /// <summary>
    /// Razor Page model for All Bookings view.
    /// cr-dotnet-1034: Replaced synchronous GridView/SqlDataSource data binding with
    /// async Task-based patterns using Entity Framework Core connected to Amazon RDS,
    /// preventing thread pool exhaustion under load and enabling efficient auto-scaling.
    ///
    /// Migrated from ASP.NET Web Forms (allbooking.aspx / allbooking.aspx.cs)
    /// to ASP.NET Core Razor Pages to enable cloud-native deployment on AWS.
    ///
    /// Replaces:
    ///   - using System.Web;                (line 5 in original allbooking.aspx.cs)
    ///   - using System.Web.UI;             (line 6 in original allbooking.aspx.cs)
    ///   - using System.Web.UI.WebControls; (line 7 in original allbooking.aspx.cs)
    ///   - public partial class allbooking : System.Web.UI.Page  (line 10 in original)
    ///   - protected void Page_Load(...)    (line 12 in original allbooking.aspx.cs)
    ///   - asp:GridView with asp:SqlDataSource (synchronous, line 15 original .aspx)
    ///   - asp:SqlDataSource SelectCommand="SELECT * FROM [booking]" (line 33 original .aspx)
    /// </summary>
    public class AllBookingModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AllBookingModel> _logger;

        public AllBookingModel(IConfiguration configuration, ILogger<AllBookingModel> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public IEnumerable<BookingViewModel> Bookings { get; set; } = new List<BookingViewModel>();
        public string StatusMessage { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }

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
        /// Creates an EF Core DbContext configured for Amazon RDS via environment variable
        /// or appsettings connection string.
        /// </summary>
        private BookingDbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<BookingDbContext>();
            optionsBuilder.UseSqlServer(GetConnectionString());
            return new BookingDbContext(optionsBuilder.Options);
        }

        /// <summary>
        /// cr-dotnet-1034: Async OnGetAsync replaces synchronous Page_Load / GridView.DataBind().
        /// Uses Entity Framework Core async query (ToListAsync) connected to Amazon RDS,
        /// preventing thread pool exhaustion and enabling efficient auto-scaling.
        /// Replaces: asp:GridView DataSourceID="SqlDataSource1" (synchronous binding, line 15 original)
        ///           asp:SqlDataSource SelectCommand="SELECT * FROM [booking]" (line 33 original)
        /// </summary>
        public async Task OnGetAsync()
        {
            await LoadBookingsAsync();
        }

        /// <summary>
        /// cr-dotnet-1034: Async data load using EF Core ToListAsync() — replaces synchronous
        /// GridView data binding via SqlDataSource. Prevents thread pool exhaustion under load.
        /// Replaces: asp:SqlDataSource SelectCommand="SELECT * FROM [booking]"
        /// </summary>
        private async Task LoadBookingsAsync()
        {
            try
            {
                using (var dbContext = CreateDbContext())
                {
                    // Async EF Core query — replaces synchronous GridView/SqlDataSource binding
                    Bookings = await dbContext.Bookings
                        .AsNoTracking()
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading bookings from database.");
                StatusMessage = "Error loading bookings. Please try again.";
                IsSuccess = false;
            }
        }
    }

    /// <summary>
    /// Entity model representing a Booking record.
    /// cr-dotnet-1034: Replaces auto-generated columns from asp:GridView DataKeyNames="TOUR_ID"
    /// in the original Web Forms page. Columns: TOUR_ID, TOUR_NAME, PLACE, Email, FirstName.
    /// </summary>
    [Table("booking")]
    public class BookingViewModel
    {
        [Key]
        [Column("TOUR_ID")]
        public int TourId { get; set; }

        [Column("TOUR_NAME")]
        public string TourName { get; set; } = string.Empty;

        [Column("PLACE")]
        public string Place { get; set; } = string.Empty;

        [Column("Email")]
        public string Email { get; set; } = string.Empty;

        [Column("FirstName")]
        public string FirstName { get; set; } = string.Empty;
    }

    /// <summary>
    /// EF Core DbContext for Booking data — replaces SqlDataSource control.
    /// cr-dotnet-1034: Enables async data access patterns via Entity Framework Core
    /// connected to Amazon RDS, preventing thread pool exhaustion under cloud load.
    /// </summary>
    public class BookingDbContext : DbContext
    {
        public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options) { }

        public DbSet<BookingViewModel> Bookings { get; set; }
    }
}
