// Migrated from Web Forms (allbooking.aspx.cs) to ASP.NET Core Razor Pages (cr-dotnet-0026)
// Changes:
//   - Removed: using System.Web.UI;                    (line 5 - Web Forms namespace)
//   - Removed: using System.Web.UI.WebControls;        (line 6 - Web Forms namespace)
//   - Removed: inheritance from System.Web.UI.Page     (line 10 - Web Forms base class)
//   - Replaced: Page_Load event handler                (line 12 - Web Forms lifecycle)
//     with OnGetAsync() method in PageModel
//   - Replaced: asp:SqlDataSource declarative data binding
//     with explicit Dapper query in OnGetAsync()
//   - Replaced: asp:GridView with strongly-typed Razor table rendering

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

        // Retrieve the RDS Proxy connection string from environment variable,
        // falling back to the appsettings.json connectionString for local development.
        private string GetConnectionString()
        {
            return Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                ?? _configuration.GetConnectionString("dbconnection")
                ?? string.Empty;
        }

        // Replaces Web Forms Page_Load event (line 12 in original code-behind)
        // and asp:SqlDataSource SelectCommand="SELECT * FROM [booking]"
        public async Task OnGetAsync()
        {
            // Use Dapper with RDS Proxy-backed SqlConnection for cloud-native connection pooling.
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string selectQuery = "SELECT TOUR_ID, TOUR_NAME, PLACE, Email, FirstName FROM [booking]";
                Bookings = await conn.QueryAsync<BookingViewModel>(selectQuery);
            }
        }
    }

    public class BookingViewModel
    {
        public int TourId { get; set; }
        public string TourName { get; set; } = string.Empty;
        public string Place { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
    }
}
