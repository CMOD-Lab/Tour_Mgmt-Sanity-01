// MIGRATION NOTE (cr-dotnet-0026):
// This file has been migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
// - Removed: System.Web.UI.Page inheritance (Web Forms base class)
// - Removed: System.Web.UI and System.Web.UI.WebControls using directives
// - Removed: System.Web using directive
// - Replaced: Page_Load event handler with ASP.NET Core Razor Pages OnGet() method
// - The PageModel class (MainProfilePageModel) replaces the Web Forms code-behind.
// - The Razor Page view is at Pages/MainProfilePage.cshtml.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Tour_Management.Pages
{
    /// <summary>
    /// Razor Pages PageModel for the Main Profile Page.
    /// Replaces the ASP.NET Web Forms MainProfilePage code-behind (System.Web.UI.Page).
    /// </summary>
    public class MainProfilePageModel : PageModel
    {
        /// <summary>
        /// Bound property for the welcome label message displayed on the page.
        /// Replaces the Web Forms Label1 server control.
        /// </summary>
        [BindProperty]
        public string WelcomeMessage { get; set; } = string.Empty;

        /// <summary>
        /// Handles GET requests. Replaces the Web Forms Page_Load event handler.
        /// </summary>
        public void OnGet()
        {
            // Page load logic preserved from Web Forms Page_Load.
            // Add any initialization logic here.
        }
    }
}
