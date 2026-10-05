// cr-dotnet-1034: Async GridView Data Binding with RDS via Entity Framework Core
// MIGRATION NOTE (cr-dotnet-0026 + cr-dotnet-1034): ASP.NET Core Razor Pages PageModel for TourCrud page.
// Migrated from ASP.NET Web Forms (TourCrud.aspx.cs) to ASP.NET Core Razor Pages.
// - Removed: System.Web, System.Web.UI, System.Web.UI.WebControls namespaces
// - Replaced: System.Web.UI.Page base class (line 18) with PageModel
// - Replaced: synchronous Page_Load + GridView.DataBind() with async Task OnGetAsync()
// - Replaced: <asp:GridView> + <asp:SqlDataSource> synchronous server controls with async EF Core + Razor table
// - cr-dotnet-1034: Replaced synchronous Dapper SqlConnection with async Entity Framework Core ToListAsync()
// - Fixed: cr-dotnet-0010 - Replaced Web.config transformation files (Web.Debug.config, Web.Release.config)
//          with environment variables and AWS Systems Manager Parameter Store for runtime configuration.
//          Configuration is injected at runtime rather than baked into build artifacts.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Tour_Management.Pages
{
    /// <summary>
    /// Razor Pages PageModel for the Tour CRUD management page.
    /// cr-dotnet-1034: Uses async Task OnGetAsync() with Entity Framework Core ToListAsync()
    /// connected to Amazon RDS, preventing thread pool exhaustion under cloud load.
    /// </summary>
    public class TourCrudModel : PageModel
    {
        /// <summary>
        /// Represents a single tour record from the database.
        /// </summary>
        public class TourRecord
        {
            public int TourId { get; set; }
            public string TourName { get; set; }
            public string Place { get; set; }
            public int Days { get; set; }
            public decimal Price { get; set; }
            public string Locations { get; set; }
            public string TourInfo { get; set; }
            public string Pic { get; set; }
        }

        /// <summary>
        /// List of all tours loaded from Amazon RDS via async EF Core query.
        /// cr-dotnet-1034: Replaces synchronous <asp:GridView> + <asp:SqlDataSource> server controls.
        /// </summary>
        public List<TourRecord> Tours { get; private set; } = new List<TourRecord>();

        // cr-dotnet-0010: Retrieve connection string at runtime from environment variable (highest priority),
        // then AWS Systems Manager Parameter Store (cloud-native secrets/config), then Web.config fallback.
        // This eliminates the need for Web.config transformation files (Web.Debug.config / Web.Release.config)
        // which bake configuration into build artifacts and are incompatible with cloud deployment pipelines.
        private static string GetConnectionString()
        {
            // 1. Environment variable — injected by AWS ECS task definition, Elastic Beanstalk, or Lambda
            var envValue = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            if (!string.IsNullOrEmpty(envValue))
                return envValue;

            // 2. AWS Systems Manager Parameter Store — runtime-configurable, no rebuild required
            //    Parameter path: /tour-mgmt/DB_CONNECTION_STRING
            //    IAM role on the compute resource must have ssm:GetParameter permission.
            try
            {
                using (var ssmClient = new AmazonSimpleSystemsManagementClient())
                {
                    var request = new GetParameterRequest
                    {
                        Name = "/tour-mgmt/DB_CONNECTION_STRING",
                        WithDecryption = true
                    };
                    var response = ssmClient.GetParameterAsync(request).GetAwaiter().GetResult();
                    if (!string.IsNullOrEmpty(response?.Parameter?.Value))
                        return response.Parameter.Value;
                }
            }
            catch
            {
                // SSM not available (e.g., local development) — fall through to Web.config
            }

            // 3. Web.config / appsettings.json fallback for local development only
            return System.Configuration.ConfigurationManager.ConnectionStrings["dbconnection"]?.ConnectionString;
        }

        private DbContextOptions<TourCrudPageDbContext> BuildDbOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TourCrudPageDbContext>();
            optionsBuilder.UseSqlServer(GetConnectionString());
            return optionsBuilder.Options;
        }

        // cr-dotnet-1034: Replaced synchronous OnGet() + GridView.DataBind() with async Task OnGetAsync()
        // Uses Entity Framework Core ToListAsync() for non-blocking Amazon RDS data access
        public async Task OnGetAsync()
        {
            await LoadToursAsync();
        }

        private async Task LoadToursAsync()
        {
            // cr-dotnet-1034: Replaces synchronous <asp:SqlDataSource SelectCommand="SELECT * FROM [Tour]">
            // with async EF Core ToListAsync() - prevents thread pool exhaustion under AWS cloud load
            using (var dbContext = new TourCrudPageDbContext(BuildDbOptions()))
            {
                Tours = await dbContext.TourRecords.ToListAsync();
            }
        }

        // cr-dotnet-1034: Async OnPostUpdateAsync replaces synchronous AutoGenerateEditButton update action
        // Replaces: UpdateCommand="UPDATE [Tour] Set [TOUR_NAME]=@TOUR_NAME,... Where [TOUR_ID]=@TOUR_ID"
        public async Task<IActionResult> OnPostUpdateAsync(int tourId, string tourName, string place,
            int days, decimal price, string locations, string tourInfo)
        {
            using (var dbContext = new TourCrudPageDbContext(BuildDbOptions()))
            {
                var tour = await dbContext.TourRecords.FindAsync(tourId);
                if (tour != null)
                {
                    tour.TourName = tourName;
                    tour.Place = place;
                    tour.Days = days;
                    tour.Price = price;
                    tour.Locations = locations;
                    tour.TourInfo = tourInfo;
                    await dbContext.SaveChangesAsync();
                }
            }
            return RedirectToPage();
        }

        // cr-dotnet-1034: Async OnPostDeleteAsync replaces synchronous AutoGenerateDeleteButton delete action
        // Replaces: DeleteCommand="Delete from [Tour] Where [TOUR_ID]=@TOUR_ID"
        public async Task<IActionResult> OnPostDeleteAsync(int tourId)
        {
            using (var dbContext = new TourCrudPageDbContext(BuildDbOptions()))
            {
                var tour = await dbContext.TourRecords.FindAsync(tourId);
                if (tour != null)
                {
                    dbContext.TourRecords.Remove(tour);
                    await dbContext.SaveChangesAsync();
                }
            }
            return RedirectToPage();
        }
    }

    // cr-dotnet-1034: Entity Framework Core DbContext for async CRUD operations on Amazon RDS
    public class TourCrudPageDbContext : DbContext
    {
        public TourCrudPageDbContext(DbContextOptions<TourCrudPageDbContext> options) : base(options) { }

        public DbSet<TourCrudModel.TourRecord> TourRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TourCrudModel.TourRecord>(entity =>
            {
                entity.ToTable("Tour");
                entity.HasKey(e => e.TourId);
                entity.Property(e => e.TourId).HasColumnName("TOUR_ID");
                entity.Property(e => e.TourName).HasColumnName("TOUR_NAME");
                entity.Property(e => e.Place).HasColumnName("PLACE");
                entity.Property(e => e.Days).HasColumnName("DAYS");
                entity.Property(e => e.Price).HasColumnName("PRICE");
                entity.Property(e => e.Locations).HasColumnName("LOCATIONS");
                entity.Property(e => e.TourInfo).HasColumnName("TOUR_INFO");
                entity.Property(e => e.Pic).HasColumnName("pic");
            });
        }
    }
}
