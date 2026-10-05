// cr-dotnet-1034: Async GridView Data Binding with RDS via Entity Framework Core
// MIGRATION NOTE (cr-dotnet-0026 + cr-dotnet-1034): This file has been migrated to ASP.NET Core Razor Pages
// with async Task-based data binding using Entity Framework Core connected to Amazon RDS.
// The Razor Pages equivalent is located at: Pages/TourCrud.cshtml and Pages/TourCrud.cshtml.cs
//
// Original Web Forms violations addressed:
//   - Line 18: System.Web.UI.Page base class replaced with Microsoft.AspNetCore.Mvc.RazorPages.PageModel
//   - System.Web.UI and System.Web.UI.WebControls namespaces removed
//   - <asp:GridView> + <asp:SqlDataSource> synchronous server controls replaced with async EF Core + Razor table
//   - Synchronous GridView.DataBind() replaced with async Task OnGetAsync() + EF Core ToListAsync()
//   - Connection string retrieved from environment variable DB_CONNECTION_STRING (Amazon RDS endpoint)
//
// Fixed: cr-dotnet-0010 - Replaced Web.config transformation files (Web.Debug.config, Web.Release.config)
//        with environment variables and AWS Systems Manager Parameter Store for runtime configuration.
//        Configuration is injected at runtime rather than baked into build artifacts, enabling
//        true infrastructure-as-code and immutable deployments.
//
// This file is retained for reference only. The active implementation is in Pages/TourCrud.cshtml.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
// Removed: using System.Web;
// Removed: using System.Web.UI;
// Removed: using System.Web.UI.WebControls;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Microsoft.EntityFrameworkCore;

namespace Tour_Management
{
    // NOTE: This class is superseded by Tour_Management.Pages.TourCrudModel (Razor Pages PageModel).
    // The System.Web.UI.Page base class has been replaced with PageModel in the migrated version.
    // cr-dotnet-1034: Synchronous GridView.DataBind() replaced with async Task OnGetAsync()
    //                 using Entity Framework Core ToListAsync() on Amazon RDS.
    public partial class TourCrud
    {
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

            // 3. Web.config fallback for local development only
            return System.Configuration.ConfigurationManager.ConnectionStrings["dbconnection"]?.ConnectionString;
        }

        // cr-dotnet-1034: Migrated Page_Load → async Task OnGetAsync() in Pages/TourCrud.cshtml.cs
        // Replaced synchronous GridView.DataBind() with async EF Core ToListAsync()
        protected void Page_Load(object sender, EventArgs e)
        {
            // Data loading migrated to Pages/TourCrud.cshtml.cs async Task OnGetAsync()
            // using Entity Framework Core ToListAsync() connected to Amazon RDS
        }

        // cr-dotnet-1034: Async data refresh using EF Core - replaces synchronous GridView.DataBind()
        public async Task RefreshDataAsync()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TourCrudDbContext>();
            optionsBuilder.UseSqlServer(GetConnectionString());

            // cr-dotnet-1034: async EF Core query replaces synchronous Dapper/GridView data binding
            using (var dbContext = new TourCrudDbContext(optionsBuilder.Options))
            {
                var tours = await dbContext.TourRecords.ToListAsync();
                // Data rendered via async Razor foreach in Pages/TourCrud.cshtml
            }
        }
    }

    // cr-dotnet-1034: Entity Framework Core DbContext for async CRUD operations on Amazon RDS
    public class TourCrudDbContext : DbContext
    {
        public TourCrudDbContext(DbContextOptions<TourCrudDbContext> options) : base(options) { }

        public DbSet<TourCrudRecord> TourRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TourCrudRecord>(entity =>
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

    public class TourCrudRecord
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
}
