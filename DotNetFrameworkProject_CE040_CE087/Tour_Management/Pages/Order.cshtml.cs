// MIGRATION NOTE (cr-dotnet-0026): ASP.NET Core Razor Pages PageModel for Order page.
// Migrated from ASP.NET Web Forms (Order.aspx.cs) to ASP.NET Core Razor Pages.
// - Removed: System.Web, System.Web.UI, System.Web.UI.WebControls namespaces
// - Replaced: System.Web.UI.Page base class with PageModel
// - Replaced: Page_Load event with OnGet() Razor Pages lifecycle method
// - Replaced: btn_click event handler with OnPost() Razor Pages POST handler
// - Replaced: TextBox.Text property access with [BindProperty] model binding
// - Replaced: Response.Redirect with RedirectToPage()
using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dapper;

namespace Tour_Management.Pages
{
    /// <summary>
    /// Razor Pages PageModel for the Order (Book Tour) page.
    /// Migrated from ASP.NET Web Forms Order code-behind.
    /// </summary>
    public class OrderModel : PageModel
    {
        // Retrieve connection string from environment variable (RDS Proxy endpoint) with fallback to configuration
        private static string GetConnectionString()
        {
            return Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                ?? System.Configuration.ConfigurationManager.ConnectionStrings["dbconnection"]?.ConnectionString;
        }

        // Migrated from Web Forms TextBox server controls to Razor Pages [BindProperty] model binding
        [BindProperty]
        public string TourName { get; set; }

        [BindProperty]
        public string City { get; set; }

        [BindProperty]
        public string Number { get; set; }

        [BindProperty]
        public string Name { get; set; }

        // OnGet replaces Page_Load for HTTP GET requests in ASP.NET Core Razor Pages
        public void OnGet()
        {
            // Page initialization logic preserved from original Page_Load
        }

        // OnPost replaces btn_click event handler for form POST submissions in ASP.NET Core Razor Pages
        public IActionResult OnPost()
        {
            string insertQuery = "insert into booking(TOUR_NAME,PLACE,Email,FirstName) values(@TOUR_NAME,@PLACE,@Email,@FirstName)";

            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Execute(insertQuery, new
                {
                    TOUR_NAME = TourName,
                    PLACE = City,
                    Email = Number,
                    FirstName = Name
                });
            }

            // Migrated from Response.Redirect("mybooking.aspx") to Razor Pages RedirectToPage
            return RedirectToPage("/mybooking");
        }
    }
}
