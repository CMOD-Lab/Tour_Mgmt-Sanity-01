// MIGRATION NOTE (cr-dotnet-0026): ASP.NET Core Razor Pages PageModel for MainProfilePage
// Migrated from ASP.NET Web Forms (MainProfilePage.aspx.cs) to ASP.NET Core Razor Pages.
// - Removed: System.Web, System.Web.UI, System.Web.UI.WebControls namespaces
// - Replaced: System.Web.UI.Page base class with PageModel
// - Replaced: Page_Load event with OnGet() Razor Pages lifecycle method
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Tour_Management.Pages
{
    /// <summary>
    /// Razor Pages PageModel for the Main Profile Page.
    /// Migrated from ASP.NET Web Forms MainProfilePage code-behind.
    /// </summary>
    public class MainProfilePageModel : PageModel
    {
        // OnGet replaces Page_Load for HTTP GET requests in ASP.NET Core Razor Pages
        public void OnGet()
        {
            // Page initialization logic preserved from original Page_Load
        }
    }
}
