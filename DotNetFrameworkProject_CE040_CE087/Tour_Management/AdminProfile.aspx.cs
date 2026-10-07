// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages (cr-dotnet-0026)
// Removed: System.Web, System.Web.UI, System.Web.UI.WebControls (Web Forms dependencies)
// Added: Microsoft.AspNetCore.Mvc, Microsoft.AspNetCore.Mvc.RazorPages (ASP.NET Core MVC)
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Tour_Management.Pages
{
    // Migrated from System.Web.UI.Page to ASP.NET Core Razor Pages PageModel (cr-dotnet-0026)
    public class AdminProfileModel : PageModel
    {
        // Replaces Web Forms Page_Load event handler
        public void OnGet()
        {
        }
    }
}
