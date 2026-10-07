// cr-dotnet-1034: Async GridView Data Binding with RDS via Entity Framework Core
// Replaced synchronous OnGet() / RefreshData() with async Task-based OnGetAsync()
// using Entity Framework Core connected to Amazon RDS, preventing thread pool exhaustion under load.
//
// MIGRATION NOTE (cr-dotnet-0026 - Web Forms Usage):
// This file has been migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
//
// Original Web Forms code-behind:
//   - Inherited from System.Web.UI.Page (line 10) — replaced with PageModel (ASP.NET Core Razor Pages)
//   - Used System.Web (line 5) — removed (Web Forms namespace)
//   - Used System.Web.UI (line 6) — removed (Web Forms namespace)
//   - Used System.Web.UI.WebControls (line 7) — removed (Web Forms namespace)
//   - Page_Load(object sender, EventArgs e) event handler — replaced with OnGetAsync() Razor Page handler
//   - <asp:GridView> DataSource / DataBind() — replaced with Bookings property populated via EF Core
//   - <asp:SqlDataSource> declarative SelectCommand — replaced with async OnGetAsync() EF Core query
//
// cr-dotnet-1034 changes:
//   - Replaced: synchronous void OnGet() with async Task OnGetAsync()
//   - Replaced: synchronous RefreshData() with async RefreshDataAsync() using EF Core ToListAsync()
//   - Replaced: Dapper SqlConnection with EF Core AllBookingDbContext (DbContext)
//   - Connection string retrieved from RDS_CONNECTION_STRING environment variable

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Tour_Management.Pages
{
    /// <summary>
    /// ASP.NET Core Razor Pages PageModel for the All Bookings admin page.
    /// Migrated from Web Forms allbooking.aspx / allbooking.aspx.cs (cr-dotnet-0026).
    /// cr-dotnet-1034: All data access converted to async EF Core patterns for Amazon RDS.
    /// </summary>
    public class AllBookingModel : PageModel
    {
        // Retrieve the RDS connection string from environment variable (Amazon RDS / RDS Proxy endpoint)
        // or fall back to appsettings.json / environment connectionStrings entry "dbconnection".
        private static string GetConnectionString()
        {
            string envConnStr = Environment.GetEnvironmentVariable("RDS_CONNECTION_STRING");
            if (!string.IsNullOrEmpty(envConnStr))
                return envConnStr;
            return System.Configuration.ConfigurationManager.ConnectionStrings["dbconnection"]?.ConnectionString
                ?? string.Empty;
        }

        // Replaces <asp:GridView> DataSource — populated in OnGetAsync() and rendered via @foreach in the view
        public IEnumerable<BookingEntity> Bookings { get; private set; } = new List<BookingEntity>();

        // cr-dotnet-1034: Replaced synchronous void OnGet() with async Task OnGetAsync()
        // Prevents thread pool exhaustion under cloud load
        public async Task OnGetAsync()
        {
            await RefreshDataAsync();
        }

        // cr-dotnet-1034: Replaced synchronous RefreshData() with async RefreshDataAsync() using EF Core
        // Replaces the implicit data binding of <asp:SqlDataSource> SelectCommand to GridView1
        private async Task RefreshDataAsync()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AllBookingDbContext>();
            optionsBuilder.UseSqlServer(GetConnectionString());

            using (var dbContext = new AllBookingDbContext(optionsBuilder.Options))
            {
                // Async EF Core query — prevents thread pool exhaustion under cloud load
                // Replaces: SelectCommand="SELECT * FROM [booking]"
                Bookings = await dbContext.Bookings
                    .AsNoTracking()
                    .ToListAsync();
            }
        }
    }

    // EF Core entity class mapping to the [booking] table in Amazon RDS
    [System.ComponentModel.DataAnnotations.Schema.Table("booking")]
    public class BookingEntity
    {
        [System.ComponentModel.DataAnnotations.Key]
        [System.ComponentModel.DataAnnotations.Schema.Column("TOUR_ID")]
        public int TourId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column("TOUR_NAME")]
        public string TourName { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Schema.Column("PLACE")]
        public string Place { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Schema.Column("Email")]
        public string Email { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Schema.Column("FirstName")]
        public string FirstName { get; set; } = string.Empty;
    }

    // EF Core DbContext for All Bookings — connects to Amazon RDS
    public class AllBookingDbContext : DbContext
    {
        public AllBookingDbContext(DbContextOptions<AllBookingDbContext> options) : base(options) { }

        public DbSet<BookingEntity> Bookings { get; set; }
    }
}
