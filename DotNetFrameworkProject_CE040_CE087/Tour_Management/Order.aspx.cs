// MIGRATION NOTE (cr-dotnet-0026):
// This file has been migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
// - Removed: System.Web, System.Web.UI, System.Web.UI.WebControls using directives (lines 7-8)
// - Removed: System.Web.UI.Page inheritance (line 12)
// - Removed: Web Forms server control field references (name, city, tour_name, number, Book, Reset)
// - Removed: Response.Write / Response.Redirect / Server.Transfer Web Forms API calls (line 14)
// - Replaced: Page_Load event with OnGet() Razor Pages handler
// - Replaced: btn_click event handler with OnPost() Razor Pages handler
// - Replaced: ConfigurationManager with IConfiguration (ASP.NET Core DI)
// - Preserved: All booking insertion business logic using Dapper + SqlConnection with RDS Proxy support
//
// cr-dotnet-0010: Replaced Web.config / ConfigurationManager connection string lookup with
// environment variable and AWS Systems Manager Parameter Store resolution.
// Web.config transformation files (Web.Debug.config, Web.Release.config) are no longer used.
// Configuration is injected at runtime via:
//   1. RDS_CONNECTION_STRING environment variable (highest priority)
//   2. AWS SSM Parameter Store key /tour-management/dbconnection (injected as SSM_DBCONNECTION env var)
//   3. IConfiguration "dbconnection" entry (local development fallback only)
// This enables immutable deployments and true infrastructure-as-code on AWS.

using System;
using System.Data.SqlClient;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace Tour_Management.Pages
{
    /// <summary>
    /// Razor Pages PageModel for the Order / Book Tour page.
    /// Replaces the ASP.NET Web Forms Order code-behind (System.Web.UI.Page).
    /// cr-dotnet-0010: Configuration resolved from environment variables / AWS SSM Parameter Store
    /// instead of Web.config / ConfigurationManager (eliminates Web.config transformation dependency).
    /// </summary>
    public class OrderModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public OrderModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Bound form fields — replace Web Forms TextBox server controls
        [BindProperty]
        public string Name { get; set; } = string.Empty;

        [BindProperty]
        public string City { get; set; } = string.Empty;

        [BindProperty]
        public string TourName { get; set; } = string.Empty;

        [BindProperty]
        public string Number { get; set; } = string.Empty;

        /// <summary>
        /// Message to display after form submission (replaces Response.Write).
        /// </summary>
        public string StatusMessage { get; set; } = string.Empty;

        /// <summary>
        /// Handles GET requests. Replaces the Web Forms Page_Load event handler.
        /// </summary>
        public void OnGet()
        {
            // Page load logic preserved from Web Forms Page_Load.
        }

        /// <summary>
        /// Handles POST requests (form submission). Replaces the Web Forms btn_click event handler.
        /// Preserves all booking insertion business logic using Dapper with RDS Proxy connection pooling.
        /// </summary>
        public IActionResult OnPost()
        {
            // cr-dotnet-0010: Retrieve the connection string from environment variables or
            // AWS Systems Manager Parameter Store — NOT from Web.config / ConfigurationManager.
            string connectionString = GetConnectionString();

            // Use Dapper with RDS Proxy connection string for cloud-native connection pooling.
            // SqlConnection is opened inside a using block to ensure proper disposal.
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string insertQuery = "insert into booking(TOUR_NAME,PLACE,Email,FirstName) values(@TOUR_NAME,@PLACE,@Email,@FirstName)";

                conn.Execute(insertQuery, new
                {
                    TOUR_NAME = TourName,
                    PLACE     = City,
                    Email     = Number,
                    FirstName = Name
                });
            }

            // Replaces Response.Redirect("mybooking.aspx") — redirect to Razor Page equivalent
            return RedirectToPage("/mybooking");
        }

        /// <summary>
        /// cr-dotnet-0010: Retrieves the database connection string from environment variables or
        /// AWS Systems Manager Parameter Store — NOT from Web.config / ConfigurationManager.
        /// Priority order:
        ///   1. RDS_CONNECTION_STRING environment variable (set in ECS task definition / Elastic Beanstalk env)
        ///   2. SSM_DBCONNECTION environment variable (injected from /tour-management/dbconnection SSM key)
        ///   3. IConfiguration "dbconnection" entry (local development fallback only)
        /// This eliminates the need for Web.Debug.config / Web.Release.config build-time transformations.
        /// </summary>
        private string GetConnectionString()
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

            // 3. Local development fallback via IConfiguration (appsettings.json)
            return _configuration.GetConnectionString("dbconnection")
                ?? throw new InvalidOperationException(
                    "Database connection string is not configured. " +
                    "Set the RDS_CONNECTION_STRING environment variable or configure the " +
                    "/tour-management/dbconnection AWS SSM Parameter Store key and inject it " +
                    "as the SSM_DBCONNECTION environment variable.");
        }
    }
}
