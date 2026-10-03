using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Dapper;

namespace Tour_Management.Pages
{
    /// <summary>
    /// Razor Page model for UserLogin - migrated from ASP.NET Web Forms (userlogin.aspx)
    /// to ASP.NET Core Razor Pages for cloud-native deployment and horizontal scalability.
    /// Replaces System.Web.UI.Page inheritance (line 12), System.Web (line 7),
    /// System.Web.UI (line 8), and System.Web.UI.WebControls using statements with a
    /// stateless Razor Page model using Dapper for cloud-native connection pooling
    /// via Amazon RDS Proxy.
    ///
    /// Replaces Web Forms patterns:
    ///   - Response.Write()       → StatusMessage property rendered in Razor view
    ///   - Response.Redirect()    → RedirectToPage() IActionResult
    ///   - Server.Transfer()      → RedirectToPage() IActionResult
    ///   - SqlConnection (direct) → Dapper with Amazon RDS Proxy connection pooling
    ///   - asp:TextBox controls   → HTML input elements with model binding
    ///   - asp:Button OnClick     → Razor Pages handler methods (OnPostLogin, OnPostSignUp)
    /// </summary>
    public class UserLoginModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public UserLoginModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string StatusMessage { get; private set; }

        private string GetConnectionString()
        {
            // Use connection string from environment variable (cloud-native 12-factor pattern)
            // or fall back to configuration for local development
            return Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                ?? _configuration.GetConnectionString("dbconnection");
        }

        public void OnGet()
        {
            // Initial page load — no action required
        }

        /// <summary>
        /// Handles POST Login requests — authenticates the user against the database.
        /// Replaces the Btn_Submit event handler from the original Web Forms code-behind.
        /// Uses Dapper with Amazon RDS Proxy connection string for cloud-native connection pooling.
        /// </summary>
        public IActionResult OnPostLogin()
        {
            // Use connection string from environment variable (cloud-native 12-factor pattern)
            // or fall back to configuration for local development
            string connectionString = GetConnectionString();

            try
            {
                // Use Dapper with Amazon RDS Proxy connection string for cloud-native connection pooling
                using (IDbConnection conn = new SqlConnection(connectionString))
                {
                    string checkPasswordQuery = "select password from Userinfo where password=@Password and email=@Email";
                    string password = conn.QueryFirstOrDefault<string>(checkPasswordQuery, new
                    {
                        Password = Password,
                        Email    = Email
                    }) ?? "";

                    if (password == Password)
                    {
                        // Replaces Response.Write("Password is correct") + Response.Redirect("MainProfilePage.aspx")
                        return RedirectToPage("/MainProfilePage");
                    }
                    else
                    {
                        // Replaces Response.Write("Password is not correct")
                        StatusMessage = "Password is not correct";
                        return Page();
                    }
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Login failed: {ex.Message}";
                return Page();
            }
        }

        /// <summary>
        /// Handles POST SignUp requests — redirects to the registration page.
        /// Replaces the Btn_reg event handler from the original Web Forms code-behind.
        /// </summary>
        public IActionResult OnPostSignUp()
        {
            // Replaces Response.Redirect("SignUpForm.aspx") + Server.Transfer("SignUpForm.aspx")
            return RedirectToPage("/SignUpForm");
        }
    }
}
