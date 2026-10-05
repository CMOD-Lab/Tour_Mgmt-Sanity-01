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
    /// Razor Page model for Tour CRUD operations.
    /// cr-dotnet-1034: Replaced synchronous GridView/SqlDataSource data binding with
    /// async Task-based patterns using Entity Framework Core connected to Amazon RDS,
    /// preventing thread pool exhaustion under load and enabling efficient auto-scaling.
    ///
    /// Migrated from ASP.NET Web Forms (TourCrud.aspx / TourCrud.aspx.cs)
    /// to ASP.NET Core Razor Pages to enable cloud-native deployment on AWS.
    ///
    /// Replaces:
    ///   - System.Web.UI (line 5 in original .aspx.cs)
    ///   - System.Web.UI.WebControls (line 6 in original .aspx.cs)
    ///   - public partial class TourCrud : System.Web.UI.Page (line 13 in original .aspx.cs)
    ///   - Page_Load / refreshdata event handlers (line 15 in original .aspx.cs)
    ///   - asp:GridView with asp:SqlDataSource (synchronous, line 13 original .aspx)
    ///   - asp:SqlDataSource SelectCommand/UpdateCommand/DeleteCommand (line 40 original .aspx)
    /// </summary>
    public class TourCrudModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TourCrudModel> _logger;

        public TourCrudModel(IConfiguration configuration, ILogger<TourCrudModel> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public IEnumerable<TourCrudViewModel> Tours { get; set; } = new List<TourCrudViewModel>();
        public string StatusMessage { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
        public int? EditTourId { get; set; }

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
        private TourCrudDbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TourCrudDbContext>();
            optionsBuilder.UseSqlServer(GetConnectionString());
            return new TourCrudDbContext(optionsBuilder.Options);
        }

        /// <summary>
        /// cr-dotnet-1034: Async OnGetAsync replaces synchronous Page_Load / GridView.DataBind().
        /// Uses Entity Framework Core async query (ToListAsync) connected to Amazon RDS,
        /// preventing thread pool exhaustion and enabling efficient auto-scaling.
        /// Replaces: asp:GridView DataSourceID="SqlDataSource1" (synchronous binding, line 13 original)
        ///           asp:SqlDataSource SelectCommand="SELECT * FROM [Tour]" (line 40 original)
        /// </summary>
        public async Task OnGetAsync(int? editId = null)
        {
            EditTourId = editId;
            await RefreshDataAsync();
        }

        /// <summary>
        /// cr-dotnet-1034: Async data load using EF Core ToListAsync() — replaces synchronous
        /// GridView data binding via SqlDataSource. Prevents thread pool exhaustion under load.
        /// </summary>
        private async Task RefreshDataAsync()
        {
            try
            {
                using (var dbContext = CreateDbContext())
                {
                    // Async EF Core query — replaces synchronous GridView/SqlDataSource binding
                    Tours = await dbContext.Tours
                        .AsNoTracking()
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading tours from database.");
                StatusMessage = "Error loading tours. Please try again.";
                IsSuccess = false;
            }
        }

        /// <summary>
        /// cr-dotnet-1034: Async OnPostUpdateAsync replaces synchronous GridView UpdateCommand.
        /// Uses EF Core async SaveChangesAsync() connected to Amazon RDS.
        /// Replaces: asp:SqlDataSource UpdateCommand="UPDATE [Tour] Set ... Where [TOUR_ID]=@TOUR_ID"
        /// </summary>
        public async Task<IActionResult> OnPostUpdateAsync(
            int editTourId,
            string tourName,
            string place,
            int days,
            decimal price,
            string locations,
            string tourInfo)
        {
            try
            {
                using (var dbContext = CreateDbContext())
                {
                    var tour = await dbContext.Tours.FindAsync(editTourId);
                    if (tour != null)
                    {
                        tour.TourName = tourName;
                        tour.Place = place;
                        tour.Days = days;
                        tour.Price = price;
                        tour.Locations = locations;
                        tour.TourInfo = tourInfo;
                        // Async SaveChangesAsync — replaces synchronous SqlDataSource UpdateCommand
                        await dbContext.SaveChangesAsync();
                    }
                }

                _logger.LogInformation("Tour updated successfully: TourId={TourId}", editTourId);
                StatusMessage = "Tour updated successfully.";
                IsSuccess = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tour: TourId={TourId}", editTourId);
                StatusMessage = "Error updating tour. Please try again.";
                IsSuccess = false;
            }

            await RefreshDataAsync();
            return Page();
        }

        /// <summary>
        /// cr-dotnet-1034: Async OnPostDeleteAsync replaces synchronous GridView DeleteCommand.
        /// Uses EF Core async SaveChangesAsync() connected to Amazon RDS.
        /// Replaces: asp:SqlDataSource DeleteCommand="Delete from [Tour] Where [TOUR_ID]=@TOUR_ID"
        /// </summary>
        public async Task<IActionResult> OnPostDeleteAsync(int deleteTourId)
        {
            try
            {
                using (var dbContext = CreateDbContext())
                {
                    var tour = await dbContext.Tours.FindAsync(deleteTourId);
                    if (tour != null)
                    {
                        dbContext.Tours.Remove(tour);
                        // Async SaveChangesAsync — replaces synchronous SqlDataSource DeleteCommand
                        await dbContext.SaveChangesAsync();
                    }
                }

                _logger.LogInformation("Tour deleted successfully: TourId={TourId}", deleteTourId);
                StatusMessage = "Tour deleted successfully.";
                IsSuccess = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tour: TourId={TourId}", deleteTourId);
                StatusMessage = "Error deleting tour. Please try again.";
                IsSuccess = false;
            }

            await RefreshDataAsync();
            return Page();
        }
    }

    /// <summary>
    /// Entity model representing a Tour record.
    /// cr-dotnet-1034: Replaces auto-generated columns from asp:GridView DataKeyNames="TOUR_ID".
    /// </summary>
    [Table("Tour")]
    public class TourCrudViewModel
    {
        [Key]
        [Column("TOUR_ID")]
        public int TourId { get; set; }

        [Column("TOUR_NAME")]
        public string TourName { get; set; } = string.Empty;

        [Column("PLACE")]
        public string Place { get; set; } = string.Empty;

        [Column("DAYS")]
        public int Days { get; set; }

        [Column("PRICE")]
        public decimal Price { get; set; }

        [Column("LOCATIONS")]
        public string Locations { get; set; } = string.Empty;

        [Column("TOUR_INFO")]
        public string TourInfo { get; set; } = string.Empty;

        [Column("pic")]
        public string Pic { get; set; } = string.Empty;
    }

    /// <summary>
    /// EF Core DbContext for Tour CRUD operations — replaces SqlDataSource control.
    /// cr-dotnet-1034: Enables async data access patterns via Entity Framework Core
    /// connected to Amazon RDS, preventing thread pool exhaustion under cloud load.
    /// </summary>
    public class TourCrudDbContext : DbContext
    {
        public TourCrudDbContext(DbContextOptions<TourCrudDbContext> options) : base(options) { }

        public DbSet<TourCrudViewModel> Tours { get; set; }
    }
}
