// MIGRATION NOTE (cr-dotnet-0026): Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages pattern.
// - Removed: using System.Web (Web Forms namespace)
// - Removed: using System.Web.UI (Web Forms Page base class)
// - Removed: using System.Web.UI.WebControls (Web Forms server controls)
// - Replaced: System.Web.UI.Page base class with Microsoft.AspNetCore.Mvc.RazorPages.PageModel
// - Replaced: Page_Load event handler with OnGet() Razor Pages lifecycle method
// - The equivalent Razor Page model is: Pages/MainProfilePage.cshtml.cs
using System;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Tour_Management
{
    // Migrated from Web Forms code-behind (System.Web.UI.Page) to ASP.NET Core Razor Pages PageModel
    public class MainProfilePage : PageModel
    {
        // OnGet replaces Page_Load for GET requests in ASP.NET Core Razor Pages
        public void OnGet()
        {
            // Page initialization logic preserved from original Page_Load
        }
    }
}
