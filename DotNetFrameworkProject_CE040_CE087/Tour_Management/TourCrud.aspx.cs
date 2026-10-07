// cr-dotnet-1034: Async GridView Data Binding with RDS via Entity Framework Core
// Replaced synchronous OnGet() / RefreshData() / OnPostUpdate() / OnPostDelete() with
// async Task-based handlers using Entity Framework Core connected to Amazon RDS,
// preventing thread pool exhaustion under load.
//
// MIGRATION NOTE (cr-dotnet-0026 - Web Forms Usage):
// This file has been migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
//
// Original Web Forms code-behind:
//   - Inherited from System.Web.UI.Page (line 13) — replaced with PageModel (ASP.NET Core Razor Pages)
//   - Used System.Web.UI (line 5) and System.Web.UI.WebControls (line 6) — removed (Web Forms namespaces)
//   - Page_Load(object sender, EventArgs e) event handler — replaced with OnGetAsync() Razor Page handler
//   - refreshdata() method bound to GridView1 server control — replaced with async EF Core query
//   - <asp:GridView> DataSource / DataBind() — replaced with @Model.Tours Razor iteration in the view
//   - <asp:SqlDataSource> declarative UpdateCommand / DeleteCommand — replaced with async OnPostUpdateAsync() / OnPostDeleteAsync()
//
// cr-dotnet-1034 changes:
//   - Replaced: synchronous void OnGet() with async Task OnGetAsync()
//   - Replaced: synchronous RefreshData() with async RefreshDataAsync() using EF Core ToListAsync()
//   - Replaced: synchronous OnPostUpdate() with async Task<IActionResult> OnPostUpdateAsync()
//   - Replaced: synchronous OnPostDelete() with async Task<IActionResult> OnPostDeleteAsync()
//   - Replaced: Dapper SqlConnection with EF Core TourCrudDbContext (DbContext)
//
// cr-dotnet-0010: Replaced Web.config / ConfigurationManager connection string lookup with
// environment variable and AWS Systems Manager Parameter Store resolution.
// Web.config transformation files (Web.Debug.config, Web.Release.config) are no longer used.
// Configuration is injected at runtime via:
//   1. RDS_CONNECTION_STRING environment variable (highest priority)
//   2. AWS SSM Parameter Store key /tour-management/dbconnection (injected as SSM_DBCONNECTION env var)
// This enables immutable deployments and true infrastructure-as-code on AWS.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Tour_Management.Pages
{
    /// <summary>
    /// ASP.NET Core Razor Pages PageModel for the Tour CRUD management page.
    /// Migrated from Web Forms TourCrud.aspx / TourCrud.aspx.cs (cr-dotnet-0026).
    /// cr-dotnet-1034: All data access converted to async EF Core patterns for Amazon RDS.
    /// cr-dotnet-0010: Configuration resolved from environment variables / AWS SSM Parameter Store
    /// instead of Web.config / ConfigurationManager (eliminates Web.config transformation dependency).
    /// </summary>
    public class TourCrudModel : PageModel
    {
        // cr-dotnet-0010: Retrieve the connection string from environment variables or
        // AWS Systems Manager Parameter Store — NOT from Web.config / ConfigurationManager.
        // Priority order:
        //   1. RDS_CONNECTION_STRING environment variable (set in ECS task definition / Elastic Beanstalk env)
        //   2. SSM_DBCONNECTION environment variable (injected from /tour-management/dbconnection SSM key)
        // This replaces the Web.config <connectionStrings> entry and eliminates the need for
        // Web.Debug.config / Web.Release.config build-time transformations.
        private static string GetConnectionString()
        {
            // 1. Check environment variable first (highest priority — set at runtime in AWS)
            string envConnStr = Environment.GetEnvironmentVariable("RDS_CONNECTION_STRING");
            if (!string.IsNullOrEmpty(envConnStr))
                return envConnStr;

            // 2. Fall back to AWS SSM Parameter Store via environment variable indirection.
            //    The SSM parameter value is injected as an environment variable by the
            //    ECS task definition or Elastic Beanstalk configuration using the SSM integration.
            //    Parameter Store key: /tour-management/dbconnection
            string ssmInjected = Environment.GetEnvironmentVariable("SSM_DBCONNECTION");
            if (!string.IsNullOrEmpty(ssmInjected))
                return ssmInjected;

            throw new InvalidOperationException(
                "Database connection string is not configured. " +
                "Set the RDS_CONNECTION_STRING environment variable or configure the " +
                "/tour-management/dbconnection AWS SSM Parameter Store key and inject it " +
                "as the SSM_DBCONNECTION environment variable.");
        }

        // Replaces <asp:GridView> DataSource — populated in OnGetAsync() and rendered via @foreach in the view
        public IEnumerable<TourCrudEntity> Tours { get; private set; } = new List<TourCrudEntity>();

        // Bind properties for the edit form (replaces <asp:GridView> AutoGenerateEditButton inline editing)
        [BindProperty]
        public int TourId { get; set; }

        [BindProperty]
        public string TourName { get; set; } = string.Empty;

        [BindProperty]
        public string Place { get; set; } = string.Empty;

        [BindProperty]
        public int Days { get; set; }

        [BindProperty]
        public decimal Price { get; set; }

        [BindProperty]
        public string Locations { get; set; } = string.Empty;

        [BindProperty]
        public string TourInfo { get; set; } = string.Empty;

        // cr-dotnet-1034: Replaced synchronous void OnGet() with async Task OnGetAsync()
        // Prevents thread pool exhaustion under cloud load
        public async Task OnGetAsync()
        {
            await RefreshDataAsync();
        }

        // cr-dotnet-1034: Replaced synchronous RefreshData() with async RefreshDataAsync() using EF Core
        private async Task RefreshDataAsync()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TourCrudDbContext>();
            optionsBuilder.UseSqlServer(GetConnectionString());

            using (var dbContext = new TourCrudDbContext(optionsBuilder.Options))
            {
                // Async EF Core query — prevents thread pool exhaustion under cloud load
                Tours = await dbContext.Tours
                    .AsNoTracking()
                    .ToListAsync();
            }
        }

        // cr-dotnet-1034: Replaced synchronous IActionResult OnPostUpdate() with async Task<IActionResult> OnPostUpdateAsync()
        // Replaces <asp:SqlDataSource> UpdateCommand handler (AutoGenerateEditButton postback)
        // Equivalent to: UPDATE [Tour] Set [TOUR_NAME]=@TOUR_NAME,[PLACE]=@PLACE,[DAYS]=@DAYS,
        //                [PRICE]=@PRICE,[LOCATIONS]=@LOCATIONS,[TOUR_INFO]=@TOUR_INFO Where [TOUR_ID]=@TOUR_ID
        public async Task<IActionResult> OnPostUpdateAsync()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TourCrudDbContext>();
            optionsBuilder.UseSqlServer(GetConnectionString());

            using (var dbContext = new TourCrudDbContext(optionsBuilder.Options))
            {
                var tour = await dbContext.Tours.FindAsync(TourId);
                if (tour != null)
                {
                    tour.TourName  = TourName;
                    tour.Place     = Place;
                    tour.Days      = Days;
                    tour.Price     = Price;
                    tour.Locations = Locations;
                    tour.TourInfo  = TourInfo;
                    await dbContext.SaveChangesAsync();
                }
            }
            return RedirectToPage();
        }

        // cr-dotnet-1034: Replaced synchronous IActionResult OnPostDelete() with async Task<IActionResult> OnPostDeleteAsync()
        // Replaces <asp:SqlDataSource> DeleteCommand handler (AutoGenerateDeleteButton postback)
        // Equivalent to: Delete from [Tour] Where [TOUR_ID]=@TOUR_ID
        public async Task<IActionResult> OnPostDeleteAsync(int tourId)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TourCrudDbContext>();
            optionsBuilder.UseSqlServer(GetConnectionString());

            using (var dbContext = new TourCrudDbContext(optionsBuilder.Options))
            {
                var tour = await dbContext.Tours.FindAsync(tourId);
                if (tour != null)
                {
                    dbContext.Tours.Remove(tour);
                    await dbContext.SaveChangesAsync();
                }
            }
            return RedirectToPage();
        }
    }

    // EF Core entity class mapping to the [Tour] table in Amazon RDS
    [System.ComponentModel.DataAnnotations.Schema.Table("Tour")]
    public class TourCrudEntity
    {
        [System.ComponentModel.DataAnnotations.Key]
        [System.ComponentModel.DataAnnotations.Schema.Column("TOUR_ID")]
        public int TourId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column("TOUR_NAME")]
        public string TourName { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Schema.Column("PLACE")]
        public string Place { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Schema.Column("DAYS")]
        public int Days { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column("PRICE")]
        public decimal Price { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column("LOCATIONS")]
        public string Locations { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Schema.Column("TOUR_INFO")]
        public string TourInfo { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Schema.Column("pic")]
        public string Pic { get; set; } = string.Empty;
    }

    // EF Core DbContext for Tour CRUD — connects to Amazon RDS
    public class TourCrudDbContext : DbContext
    {
        public TourCrudDbContext(DbContextOptions<TourCrudDbContext> options) : base(options) { }

        public DbSet<TourCrudEntity> Tours { get; set; }
    }
}
