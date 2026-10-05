// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages (cr-dotnet-0026)
// Replaced: System.Web.UI, System.Web.UI.WebControls, System.Web.UI.Page inheritance, Page_Load event handler
// Migrated to: Microsoft.AspNetCore.Mvc.RazorPages.PageModel with OnGet() lifecycle method

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Tour_Management.Pages
{
    public class AdminProfileModel : PageModel
    {
        private readonly ILogger<AdminProfileModel> _logger;

        public AdminProfileModel(ILogger<AdminProfileModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            // Page load logic migrated from Web Forms Page_Load event handler
        }
    }
}
