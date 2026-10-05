// cr-dotnet-1034: Async GridView Data Binding with RDS via Entity Framework Core
// MIGRATION NOTE (cr-dotnet-0026 + cr-dotnet-1034): ASP.NET Core Razor Pages PageModel for AllBooking page.
// Migrated from ASP.NET Web Forms (allbooking.aspx / allbooking.aspx.cs) to ASP.NET Core Razor Pages.
// - Removed: System.Web (line 5), System.Web.UI (line 6), System.Web.UI.WebControls (line 6) namespaces
// - Removed: System.Web.UI.Page base class (line 10) and System.Web.UI.WebControls references (line 12)
// - Replaced: System.Web.UI.Page base class with PageModel
// - Replaced: synchronous Page_Load + GridView.DataBind() with async Task OnGetAsync()
// - cr-dotnet-1034: Replaced synchronous Dapper SqlConnection with async Entity Framework Core ToListAsync()
// - Replaced: <asp:GridView> + <asp:SqlDataSource> server controls with async EF Core + Razor foreach
// - Replaced: SqlDataSource ConnectionString binding with environment variable DB_CONNECTION_STRING (Amazon RDS)
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Tour_Management.Pages
{
    /// <summary>
    /// Razor Pages PageModel for the All Bookings page.
    /// cr-dotnet-1034: Uses async Task OnGetAsync() with Entity Framework Core ToListAsync()
    /// connected to Amazon RDS, preventing thread pool exhaustion under cloud load.
    /// </summary>
    public class AllBookingModel : PageModel
    {
        /// <summary>
        /// Represents a single booking record from the database.
        /// </summary>
        public class BookingRecord
        {
            public int TourId { get; set; }
            public string TourName { get; set; }
            public string Place { get; set; }
            public string Email { get; set; }
            public string FirstName { get; set; }
        }

        /// <summary>
        /// List of all bookings loaded from Amazon RDS via async EF Core query.
        /// cr-dotnet-1034: Replaces synchronous <asp:GridView> + <asp:SqlDataSource> server controls.
        /// </summary>
        public List<BookingRecord> Bookings { get; private set; } = new List<BookingRecord>();

        // Retrieve connection string from environment variable (Amazon RDS Proxy endpoint)
        private static string GetConnectionString()
        {
            return Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                ?? System.Configuration.ConfigurationManager.ConnectionStrings["dbconnection"]?.ConnectionString;
        }

        private DbContextOptions<AllBookingPageDbContext> BuildDbOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AllBookingPageDbContext>();
            optionsBuilder.UseSqlServer(GetConnectionString());
            return optionsBuilder.Options;
        }

        // cr-dotnet-1034: Replaced synchronous OnGet() + GridView.DataBind() with async Task OnGetAsync()
        // Uses Entity Framework Core ToListAsync() for non-blocking Amazon RDS data access
        public async Task OnGetAsync()
        {
            // cr-dotnet-1034: Replaces synchronous <asp:SqlDataSource SelectCommand="SELECT * FROM [booking]">
            // with async EF Core ToListAsync() - prevents thread pool exhaustion under AWS cloud load
            using (var dbContext = new AllBookingPageDbContext(BuildDbOptions()))
            {
                Bookings = await dbContext.BookingRecords.ToListAsync();
            }
        }
    }

    // cr-dotnet-1034: Entity Framework Core DbContext for async data access to Amazon RDS
    public class AllBookingPageDbContext : DbContext
    {
        public AllBookingPageDbContext(DbContextOptions<AllBookingPageDbContext> options) : base(options) { }

        public DbSet<AllBookingModel.BookingRecord> BookingRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AllBookingModel.BookingRecord>(entity =>
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
}
