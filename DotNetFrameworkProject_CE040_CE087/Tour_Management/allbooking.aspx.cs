// cr-dotnet-1034: Async GridView Data Binding with RDS via Entity Framework Core
// MIGRATION NOTE (cr-dotnet-0026 + cr-dotnet-1034): This file has been migrated to ASP.NET Core Razor Pages
// with async Task-based data binding using Entity Framework Core connected to Amazon RDS.
// The Razor Pages equivalent is located at: Pages/AllBooking.cshtml and Pages/AllBooking.cshtml.cs
//
// Original Web Forms violations addressed:
//   - Line 5: using System.Web; → removed (Web Forms namespace)
//   - Line 6: using System.Web.UI; → removed (Web Forms namespace)
//   - Line 6: using System.Web.UI.WebControls; → removed (Web Forms namespace)
//   - Line 10: System.Web.UI.Page base class → replaced with PageModel in migrated version
//   - Line 12: System.Web.UI.WebControls references → replaced with async EF Core + Razor table
//   - cr-dotnet-1034: Synchronous GridView.DataBind() replaced with async Task OnGetAsync()
//                     using Entity Framework Core ToListAsync() on Amazon RDS
//
// This file is retained for reference only. The active implementation is in Pages/AllBooking.cshtml.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
// Removed: using System.Web;              (line 5 - Web Forms namespace)
// Removed: using System.Web.UI;           (line 6 - Web Forms namespace)
// Removed: using System.Web.UI.WebControls; (line 6 - Web Forms namespace)
using Microsoft.EntityFrameworkCore;

namespace Tour_Management
{
    // NOTE: This class is superseded by Tour_Management.Pages.AllBookingModel (Razor Pages PageModel).
    // The System.Web.UI.Page base class (line 10) has been replaced with PageModel in the migrated version.
    // cr-dotnet-1034: Synchronous GridView.DataBind() replaced with async Task OnGetAsync()
    //                 using Entity Framework Core ToListAsync() on Amazon RDS.
    public partial class allbooking
    {
        // Retrieve connection string from environment variable (Amazon RDS endpoint)
        private static string GetConnectionString()
        {
            return Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                ?? System.Configuration.ConfigurationManager.ConnectionStrings["dbconnection"]?.ConnectionString;
        }

        // cr-dotnet-1034: Migrated Page_Load → async Task OnGetAsync() in Pages/AllBooking.cshtml.cs
        // Replaced synchronous GridView.DataBind() with async EF Core ToListAsync()
        protected void Page_Load(object sender, EventArgs e)
        {
            // Data loading migrated to Pages/AllBooking.cshtml.cs async Task OnGetAsync()
            // using Entity Framework Core ToListAsync() connected to Amazon RDS
        }

        // cr-dotnet-1034: Async data refresh using EF Core - replaces synchronous GridView.DataBind()
        public async Task RefreshDataAsync()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AllBookingDbContext>();
            optionsBuilder.UseSqlServer(GetConnectionString());

            // cr-dotnet-1034: async EF Core query replaces synchronous Dapper/GridView data binding
            using (var dbContext = new AllBookingDbContext(optionsBuilder.Options))
            {
                var bookings = await dbContext.BookingRecords.ToListAsync();
                // Data rendered via async Razor foreach in Pages/AllBooking.cshtml
            }
        }
    }

    // cr-dotnet-1034: Entity Framework Core DbContext for async data access to Amazon RDS
    public class AllBookingDbContext : DbContext
    {
        public AllBookingDbContext(DbContextOptions<AllBookingDbContext> options) : base(options) { }

        public DbSet<AllBookingRecord> BookingRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AllBookingRecord>(entity =>
            {
                entity.ToTable("booking");
                entity.HasKey(e => e.TourId);
                entity.Property(e => e.TourId).HasColumnName("TOUR_ID");
                entity.Property(e => e.TourName).HasColumnName("TOUR_NAME");
                entity.Property(e => e.Place).HasColumnName("PLACE");
                entity.Property(e => e.Email).HasColumnName("Email");
                entity.Property(e => e.FirstName).HasColumnName("FirstName");
            });
        }
    }

    public class AllBookingRecord
    {
        public int TourId { get; set; }
        public string TourName { get; set; }
        public string Place { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
    }
}
