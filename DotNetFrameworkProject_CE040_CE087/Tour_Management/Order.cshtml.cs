using System;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace Tour_Management.Pages
{
    /// <summary>
    /// Razor Page model for Order - migrated from ASP.NET Web Forms (Order.aspx / Order.aspx.cs)
    /// to ASP.NET Core Razor Pages for cloud-native deployment and horizontal scalability.
    ///
    /// Migration changes (Rule cr-dotnet-0026 — Web Forms Usage):
    ///   Line 7:  "using System.Web;"               — removed; not available in ASP.NET Core.
    ///   Line 8:  "using System.Web.UI;"             — removed; replaced by RazorPages PageModel.
    ///   Line 12: "public partial class Order : System.Web.UI.Page" — replaced by PageModel.
    ///   Line 14: "protected void btn_click(object sender, EventArgs e)" — replaced by OnPostRegister().
    ///
    /// Replaces Web Forms server controls (asp:TextBox, asp:Button, asp:Label, runat="server")
    /// with standard HTML form fields and Razor Page handler methods.
    /// Uses Dapper with Amazon RDS Proxy connection string for cloud-native connection pooling.
    /// </summary>
    public class OrderModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public OrderModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Form-bound properties replacing asp:TextBox Web Forms server controls
        [BindProperty]
        public string FirstName { get; set; }

        [BindProperty]
        public string City { get; set; }

        [BindProperty]
        public string TourName { get; set; }

        [BindProperty]
        public string MobileNumber { get; set; }

        /// <summary>
        /// Status message displayed after form submission (replaces Response.Write in Web Forms).
        /// </summary>
        public string StatusMessage { get; private set; }

        /// <summary>
        /// Handles GET requests. Replaces the Web Forms Page_Load event handler.
        /// </summary>
        public void OnGet()
        {
            // No initialization required for GET — form fields start empty.
        }

        /// <summary>
        /// Handles POST form submission for the "Register" button.
        /// Replaces the Web Forms btn_click event handler (Order.aspx.cs line 14).
        /// Uses Dapper with Amazon RDS Proxy connection string for cloud-native connection pooling.
        /// Replaces direct SqlConnection / SqlCommand pattern with Dapper Execute.
        /// </summary>
        public IActionResult OnPostRegister()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Use connection string from environment variable (cloud-native / 12-factor pattern)
            // or fall back to configuration for local development.
            string connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                ?? _configuration.GetConnectionString("dbconnection");

            try
            {
                // Use Dapper with Amazon RDS Proxy connection string for cloud-native connection pooling.
                // Replaces: SqlConnection conn = new SqlConnection(...); conn.Open(); SqlCommand com = ...
                using (IDbConnection conn = new SqlConnection(connectionString))
                {
                    const string insertQuery =
                        "INSERT INTO booking (TOUR_NAME, PLACE, Email, FirstName) " +
                        "VALUES (@TOUR_NAME, @PLACE, @Email, @FirstName)";

                    conn.Execute(insertQuery, new
                    {
                        TOUR_NAME = TourName,
                        PLACE = City,
                        Email = MobileNumber,
                        FirstName = FirstName
                    });
                }

                // Replaces: Response.Redirect("mybooking.aspx") — Web Forms redirect pattern.
                return RedirectToPage("/mybooking");
            }
            catch (Exception ex)
            {
                StatusMessage = $"Registration failed: {ex.Message}";
                return Page();
            }
        }
    }
}
