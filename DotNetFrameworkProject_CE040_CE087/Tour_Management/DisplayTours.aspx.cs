// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages (cr-dotnet-0026)
// Removed: System.Web, System.Web.UI, System.Web.UI.WebControls, System.Data.SqlClient (Web Forms dependencies)
// Added: Microsoft.AspNetCore.Mvc.RazorPages, Microsoft.Data.SqlClient, Dapper (ASP.NET Core MVC)
//
// cr-dotnet-1034: Replaced synchronous GridView data binding with async Task-based pattern
// using Entity Framework Core / Dapper connected to Amazon RDS, preventing thread pool
// exhaustion under load and enabling efficient auto-scaling in cloud deployments.
// Changes:
//   - Replaced: synchronous OnGet() method (Web Forms Page_Load equivalent)
//     with async Task OnGetAsync() to prevent thread pool exhaustion
//   - Replaced: synchronous conn.Query<TourItem>() Dapper call
//     with async await conn.QueryAsync<TourItem>() for non-blocking I/O
//   - Added: System.Threading.Tasks using directive for Task-based async pattern
//   - Added: ILogger<DisplayToursModel> for structured cloud monitoring

using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Tour_Management.Pages
{
    // Migrated from System.Web.UI.Page to ASP.NET Core Razor Pages PageModel (cr-dotnet-0026)
    // cr-dotnet-1034: Uses async Task OnGetAsync() for cloud-scalable non-blocking data access
    public class DisplayToursModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DisplayToursModel> _logger;

        public DisplayToursModel(IConfiguration configuration, ILogger<DisplayToursModel> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public List<TourItem> Tours { get; set; } = new List<TourItem>();

        // Retrieve the RDS Proxy connection string from environment variable,
        // falling back to the appsettings.json connectionString for local development.
        private string GetConnectionString()
        {
            return Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                ?? _configuration.GetConnectionString("dbconnection")
                ?? string.Empty;
        }

        // cr-dotnet-1034: Replaces synchronous Web Forms Page_Load / GridView.DataBind() pattern.
        // Uses async Task OnGetAsync() with await conn.QueryAsync<>() via Dapper + Amazon RDS
        // to prevent thread pool exhaustion and enable cloud auto-scaling under load.
        public async Task OnGetAsync()
        {
            try
            {
                // Use Dapper with RDS Proxy-backed SqlConnection for cloud-native connection pooling.
                using (IDbConnection conn = new SqlConnection(GetConnectionString()))
                {
                    Tours = (await conn.QueryAsync<TourItem>(
                        "SELECT TOUR_NAME AS TourName, pic AS Pic, PRICE AS Price, DAYS AS Days, LOCATIONS AS Locations, TOUR_ID AS TourId FROM [Tour]"
                    )).AsList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading tours from Amazon RDS.");
                Tours = new List<TourItem>();
            }
        }
    }

    public class TourItem
    {
        public string TourName { get; set; } = string.Empty;
        public string Pic { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;
        public string Days { get; set; } = string.Empty;
        public string Locations { get; set; } = string.Empty;
        public int TourId { get; set; }
    }
}
