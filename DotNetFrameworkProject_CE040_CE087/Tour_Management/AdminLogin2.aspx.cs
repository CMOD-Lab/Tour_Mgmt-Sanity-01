// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages (cr-dotnet-0026)
// Removed: System.Web, System.Web.UI, System.Web.UI.WebControls (Web Forms dependencies)
// Added: Microsoft.AspNetCore.Mvc, Microsoft.AspNetCore.Mvc.RazorPages (ASP.NET Core MVC/Razor Pages)
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace Tour_Management.Pages
{
    // Migrated from System.Web.UI.Page to ASP.NET Core Razor Pages PageModel (cr-dotnet-0026)
    public class AdminLogin2Model : PageModel
    {
        private readonly IConfiguration _configuration;

        public AdminLogin2Model(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        [Required]
        public string Email { get; set; }

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            // Page load handler - no initialization required
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Retrieve admin credentials from environment variables for cloud-native security
            string adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL")
                ?? _configuration["AdminCredentials:Email"]
                ?? "admin@gmail.com";
            string adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD")
                ?? _configuration["AdminCredentials:Password"]
                ?? "admin";

            if (Password == adminPassword && Email == adminEmail)
            {
                return RedirectToPage("/AdminProfile");
            }

            ErrorMessage = "Invalid credentials. Please try again.";
            return Page();
        }
    }
}
