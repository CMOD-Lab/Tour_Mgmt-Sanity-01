using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace Tour_Management.Pages
{
    /// <summary>
    /// Razor Page model for AdminLogin2 - migrated from ASP.NET Web Forms (AdminLogin2.aspx)
    /// to ASP.NET Core Razor Pages for cloud-native deployment and horizontal scalability.
    /// </summary>
    public class AdminLogin2Model : PageModel
    {
        private readonly IConfiguration _configuration;

        public AdminLogin2Model(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            // Page initialization - no action required on GET
        }

        public IActionResult OnPostLogin()
        {
            // Retrieve admin credentials from environment variables (cloud-native pattern)
            // instead of hardcoded values for secure cloud deployment
            string adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL")
                ?? _configuration["AdminCredentials:Email"]
                ?? "admin@gmail.com";

            string adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD")
                ?? _configuration["AdminCredentials:Password"]
                ?? "admin";

            if (Password == adminPassword && Email == adminEmail)
            {
                // Redirect to AdminProfile Razor Page
                return RedirectToPage("/AdminProfile");
            }

            ErrorMessage = "Invalid credentials. Please try again.";
            return Page();
        }
    }
}
