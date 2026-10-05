// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages (cr-dotnet-0026)
// Removed: System.Web, System.Web.UI, System.Web.UI.WebControls (Web Forms dependencies)
// Added: Microsoft.AspNetCore.Mvc.RazorPages (ASP.NET Core Razor Pages)
using System;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Tour_Management.Pages
{
    // Migrated from System.Web.UI.Page to PageModel (ASP.NET Core Razor Pages)
    public class AdminProfileModel : PageModel
    {
        public void OnGet()
        {
            // Page load logic migrated from Page_Load event handler to OnGet method
        }
    }
}
