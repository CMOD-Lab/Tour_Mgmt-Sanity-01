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
    /// Razor Page model for SignUpForm - migrated from ASP.NET Web Forms (SignUpForm.aspx)
    /// to ASP.NET Core Razor Pages for cloud-native deployment and horizontal scalability.
    /// Replaces System.Web.UI.Page inheritance and Web Forms server controls with
    /// stateless Razor Page model using Dapper for cloud-native connection pooling
    /// via Amazon RDS Proxy.
    /// </summary>
    public class SignUpFormModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public SignUpFormModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Bound properties for form fields
        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string FirstName { get; set; }

        [BindProperty]
        public string LastName { get; set; }

        [BindProperty]
        public string Gender { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        [BindProperty]
        public string Dob { get; set; }

        [BindProperty]
        public string Street { get; set; }

        [BindProperty]
        public string City { get; set; }

        [BindProperty]
        public string State { get; set; }

        public string StatusMessage { get; private set; }

        public void OnGet()
        {
            // Initial page load — no action required
        }

        public IActionResult OnPostRegister()
        {
            if (!ModelState.IsValid)
            {
                StatusMessage = "Please fill in all required fields.";
                return Page();
            }

            // Use connection string from environment variable (cloud-native 12-factor pattern)
            // or fall back to configuration for local development
            string connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                ?? _configuration.GetConnectionString("dbconnection");

            try
            {
                // Use Dapper with Amazon RDS Proxy connection string for cloud-native connection pooling
                using (IDbConnection conn = new SqlConnection(connectionString))
                {
                    const string insertQuery =
                        "INSERT INTO UserInfo(Email, FirstName, LastName, Gender, Password, dob, Street, City, State) " +
                        "VALUES (@email, @FirstName, @LastName, @Gender, @Password, @dob, @Street, @City, @State)";

                    conn.Execute(insertQuery, new
                    {
                        email    = Email,
                        FirstName = FirstName,
                        LastName  = LastName,
                        Gender    = Gender,
                        Password  = Password,
                        dob       = Dob,
                        Street    = Street,
                        City      = City,
                        State     = State
                    });
                }

                // Redirect to login page after successful registration
                return RedirectToPage("/userlogin");
            }
            catch (Exception ex)
            {
                StatusMessage = $"Registration failed: {ex.Message}";
                return Page();
            }
        }
    }
}
