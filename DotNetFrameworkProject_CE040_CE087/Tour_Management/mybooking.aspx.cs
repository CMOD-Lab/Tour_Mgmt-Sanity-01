// cr-dotnet-1034: Async GridView Data Binding with RDS via Entity Framework Core
// MIGRATION NOTE (cr-dotnet-0026 + cr-dotnet-1034): This file has been migrated to ASP.NET Core Razor Pages
// with async Task-based data binding using Entity Framework Core connected to Amazon RDS.
// The Razor Pages equivalent is located at: Pages/MyBooking.cshtml and Pages/MyBooking.cshtml.cs
//
// Original Web Forms violations addressed:
//   - Line 4: using System.Web;                → removed (Web Forms namespace)
//   - Line 5: using System.Web.UI;             → removed (Web Forms namespace)
//   - Line 6: using System.Web.UI.WebControls; → removed (Web Forms namespace)
//   - Line 10: System.Web.UI.Page base class   → replaced with PageModel in Pages/MyBooking.cshtml.cs
//   - Line 12: public partial class mybooking : System.Web.UI.Page → superseded by MyBookingModel : PageModel
//   - cr-dotnet-1034: Synchronous GridView.DataBind() replaced with async Task OnGetAsync()
//                     using Entity Framework Core ToListAsync() on Amazon RDS
//
// This file is retained for reference only. The active implementation is in Pages/MyBooking.cshtml.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
// Removed: using System.Web;                (Web Forms namespace - cr-dotnet-0026)
// Removed: using System.Web.UI;             (Web Forms namespace - cr-dotnet-0026)
// Removed: using System.Web.UI.WebControls; (Web Forms namespace - cr-dotnet-0026)
using Microsoft.EntityFrameworkCore;

namespace Tour_Management
{
    // NOTE: This class is superseded by Tour_Management.Pages.MyBookingModel (Razor Pages PageModel).
    // The System.Web.UI.Page base class has been replaced with PageModel in the migrated version.
    // cr-dotnet-1034: Synchronous GridView.DataBind() replaced with async Task OnGetAsync()
    //                 using Entity Framework Core ToListAsync() on Amazon RDS.
    public partial class mybooking
    {
        // Retrieve connection string from environment variable (Amazon RDS endpoint)
        private static string GetConnectionString()
        {
            return Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                ?? System.Configuration.ConfigurationManager.ConnectionStrings["dbconnection"]?.ConnectionString;
        }

        // cr-dotnet-1034: Migrated Page_Load → async Task OnGetAsync() in Pages/MyBooking.cshtml.cs
        // Replaced synchronous GridView.DataBind() with async EF Core ToListAsync()
        protected void Page_Load(object sender, EventArgs e)
        {
            // Data loading migrated to Pages/MyBooking.cshtml.cs async Task OnGetAsync()
            // using Entity Framework Core ToListAsync() connected to Amazon RDS
        }

        // cr-dotnet-1034: Async data refresh using EF Core - replaces synchronous GridView.DataBind()
        public async Task RefreshDataAsync()
        {
            var optionsBuilder = new DbContextOptionsBuilder<MyBookingDbContext>();
            optionsBuilder.UseSqlServer(GetConnectionString());

            // cr-dotnet-1034: async EF Core query replaces synchronous Dapper/GridView data binding
            using (var dbContext = new MyBookingDbContext(optionsBuilder.Options))
            {
                var bookings = await dbContext.BookingRecords.ToListAsync();
                // Data rendered via async Razor foreach in Pages/MyBooking.cshtml
            }
        }
    }

    // cr-dotnet-1034: Entity Framework Core DbContext for async data access to Amazon RDS
    public class MyBookingDbContext : DbContext
    {
        public MyBookingDbContext(DbContextOptions<MyBookingDbContext> options) : base(options) { }

        public DbSet<MyBookingRecord> BookingRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MyBookingRecord>(entity =>
            {
                entity.ToTable("booking");
                entity.HasKey(e => e.TourId);
                entity.Property(e => e.TourId).HasColumnName("TOUR_ID");
                entity.Property(e => e.TourName).HasColumnName("TOUR_NAME");
            });
        }
    }

    public class MyBookingRecord
    {
        public int TourId { get; set; }
        public string TourName { get; set; }
    }
}
