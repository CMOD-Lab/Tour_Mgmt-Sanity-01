// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages (cr-dotnet-0026)
// Removed: System.Web, System.Web.UI, System.Web.UI.WebControls (Web Forms dependencies)
// Added: Microsoft.AspNetCore.Mvc, Microsoft.AspNetCore.Mvc.RazorPages (ASP.NET Core MVC)
// cr-dotnet-0010: Replaced Web.config / ConfigurationManager connection string lookup with
//                 Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") so that
//                 configuration is injected at runtime (AWS ECS/EB environment variable or
//                 AWS Systems Manager Parameter Store) rather than baked into build artifacts
//                 via Web.Debug.config / Web.Release.config XDT transformations.
using System;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
// cr-dotnet-0010: IConfiguration used as fallback for local development only;
// in AWS cloud environments DB_CONNECTION_STRING environment variable takes precedence.
using Microsoft.Extensions.Configuration;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Tour_Management.Pages
{
    // Migrated from System.Web.UI.Page to ASP.NET Core Razor Pages PageModel (cr-dotnet-0026)
    public class OrderModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public OrderModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public string Name { get; set; }

        [BindProperty]
        public string City { get; set; }

        [BindProperty]
        public string TourName { get; set; }

        [BindProperty]
        public string Number { get; set; }

        public string StatusMessage { get; set; }

        // cr-dotnet-0010: Retrieve the connection string from the DB_CONNECTION_STRING
        // environment variable (set in AWS ECS task definition, Elastic Beanstalk
        // environment properties, or injected from AWS Systems Manager Parameter Store).
        // Falls back to appsettings.json / Web.config connectionString for local development.
        // Web.Debug.config and Web.Release.config XDT transforms are no longer used.
        private string GetConnectionString()
        {
            return Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                ?? _configuration.GetConnectionString("dbconnection");
        }

        // Replaces Web Forms Page_Load event handler
        public void OnGet()
        {
        }

        // Replaces Web Forms btn_click event handler
        public IActionResult OnPost()
        {
            // Use Dapper with RDS Proxy-backed SqlConnection for cloud-native connection pooling.
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string insertQuery = "insert into booking(TOUR_NAME,PLACE,Email,FirstName) values(@TOUR_NAME,@PLACE,@Email,@FirstName)";

                conn.Execute(insertQuery, new
                {
                    TOUR_NAME = TourName,
                    PLACE     = City,
                    Email     = Number,
                    FirstName = Name
                });

                StatusMessage = "Registration Successful";
                // Replaces Response.Redirect with ASP.NET Core redirect
                return RedirectToPage("/mybooking");
            }
        }
    }
}
