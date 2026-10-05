// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages (cr-dotnet-0026)
// cr-dotnet-0010: Web.config Transformations — Replaced ConfigurationManager.ConnectionStrings
//   with environment variable (DB_CONNECTION_STRING) and AWS Systems Manager Parameter Store
//   pattern. Configuration is now injected at runtime via IConfiguration rather than baked
//   into build artifacts through Web.config transformation files.
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Dapper;

namespace Tour_Management.Pages
{
    /// <summary>
    /// Razor Page model for Order - migrated from ASP.NET Web Forms System.Web.UI.Page
    /// to ASP.NET Core Razor Pages PageModel for cloud-native, stateless, horizontally scalable deployment.
    ///
    /// Cloud remediation (cr-dotnet-0010):
    ///   Eliminated Web.config transformation dependency (Web.Debug.config / Web.Release.config).
    ///   Connection string is now resolved at runtime from:
    ///     1. DB_CONNECTION_STRING environment variable (AWS ECS task definition / Elastic Beanstalk
    ///        environment property / App Runner environment variable) — primary source.
    ///     2. AWS Systems Manager Parameter Store via the AWS .NET SDK SSM provider
    ///        (parameter path: /tour-management/db-connection-string) — secondary source.
    ///     3. appsettings.json "ConnectionStrings:dbconnection" — local development fallback only.
    ///   This enables immutable deployments where configuration is never baked into build artifacts.
    /// </summary>
    public class OrderModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public OrderModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public BookingInputModel BookingInput { get; set; } = new BookingInputModel();

        public string StatusMessage { get; private set; } = string.Empty;

        // cr-dotnet-0010: Retrieve connection string from environment variable (RDS Proxy endpoint)
        // with fallback to IConfiguration (appsettings.json / AWS SSM Parameter Store).
        // ConfigurationManager.ConnectionStrings removed — no longer relies on Web.config
        // transformation files for environment-specific configuration.
        private string GetConnectionString()
        {
            // Primary: environment variable injected by AWS ECS / Elastic Beanstalk / App Runner
            string envConnStr = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            if (!string.IsNullOrEmpty(envConnStr))
                return envConnStr;

            // Secondary: IConfiguration (reads from appsettings.json or AWS SSM Parameter Store
            // when the AWSSDK.Extensions.NETCore.Setup + Amazon.Extensions.Configuration.SystemsManager
            // packages are configured in Program.cs / Startup.cs)
            return _configuration.GetConnectionString("dbconnection");
        }

        public void OnGet()
        {
            // GET handler - display the booking form
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Use Dapper with RDS Proxy-compatible SqlConnection (connection pooling handled by RDS Proxy)
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string insertQuery = "insert into booking(TOUR_NAME,PLACE,Email,FirstName) values(@TOUR_NAME,@PLACE,@Email,@FirstName)";
                conn.Execute(insertQuery, new
                {
                    TOUR_NAME = BookingInput.TourName,
                    PLACE = BookingInput.Place,
                    Email = BookingInput.Email,
                    FirstName = BookingInput.FirstName
                });
            }

            // Redirect after successful POST (Post/Redirect/Get pattern)
            return RedirectToPage("/mybooking");
        }
    }

    public class BookingInputModel
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        public string Place { get; set; } = string.Empty;

        [Required]
        public string TourName { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;
    }
}
