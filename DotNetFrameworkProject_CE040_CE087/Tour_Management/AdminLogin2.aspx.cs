// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages (cr-dotnet-0026)
// Removed: System.Web, System.Web.UI, System.Web.UI.WebControls (Web Forms dependencies)
// Replaced: System.Web.UI.Page inheritance with Razor PageModel
// Replaced: Response.Redirect / Server.Transfer with RedirectToPage
// Replaced: Page_Load event handler with OnGet / OnPost Razor Page handlers
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Tour_Management.Pages
{
    public class AdminLogin2Model : PageModel
    {
        private readonly ILogger<AdminLogin2Model> _logger;

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public AdminLogin2Model(ILogger<AdminLogin2Model> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            // Page load - no action required
        }

        public IActionResult OnPost()
        {
            // Validate admin credentials.
            // NOTE: Credentials should be stored in environment variables or AWS Secrets Manager
            // rather than hardcoded values for production cloud deployments.
            string adminEmail    = System.Environment.GetEnvironmentVariable("ADMIN_EMAIL")    ?? "admin@gmail.com";
            string adminPassword = System.Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? "admin";

            if (Password == adminPassword && Email == adminEmail)
            {
                _logger.LogInformation("Admin login successful for '{Email}'.", Email);
                return RedirectToPage("/AdminProfile");
            }

            _logger.LogWarning("Failed admin login attempt for '{Email}'.", Email);
            ErrorMessage = "Invalid email or password.";
            return Page();
        }
    }
}
