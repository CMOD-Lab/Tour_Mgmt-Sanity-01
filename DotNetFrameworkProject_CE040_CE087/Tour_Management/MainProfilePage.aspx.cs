// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages (cr-dotnet-0026)
using System;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Tour_Management.Pages
{
    /// <summary>
    /// Razor Page model for MainProfilePage - migrated from ASP.NET Web Forms System.Web.UI.Page
    /// to ASP.NET Core Razor Pages PageModel for cloud-native, stateless, horizontally scalable deployment.
    /// </summary>
    public class MainProfilePageModel : PageModel
    {
        public string WelcomeMessage { get; private set; } = string.Empty;

        public void OnGet()
        {
            // Page load logic migrated from Web Forms Page_Load event handler
            // Stateless: no ViewState, no server-side postback lifecycle
        }
    }
}
