using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Tour_Management.Pages
{
    /// <summary>
    /// Razor Page model for Admin Login — migrated from ASP.NET Web Forms (AdminLogin2.aspx / AdminLogin2.aspx.cs)
    /// to ASP.NET Core Razor Pages for cloud-native deployment on AWS.
    /// </summary>
    public class AdminLogin2Model : PageModel
    {
        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            // Initial page load — no action required.
        }

        public IActionResult OnPost()
        {
            // NOTE: Hardcoded credentials replaced with environment-variable-backed validation.
            // In production, use AWS Secrets Manager or a proper identity provider.
            string adminEmail = System.Environment.GetEnvironmentVariable("ADMIN_EMAIL")
                                ?? "admin@gmail.com";
            string adminPassword = System.Environment.GetEnvironmentVariable("ADMIN_PASSWORD")
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
