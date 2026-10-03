using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Tour_Management.Pages
{
    /// <summary>
    /// Razor Page model for MainProfilePage - migrated from ASP.NET Web Forms (MainProfilePage.aspx)
    /// to ASP.NET Core Razor Pages for cloud-native deployment and horizontal scalability.
    /// Replaces System.Web.UI.Page base class and Web Forms code-behind pattern with
    /// the lightweight ASP.NET Core Razor Pages PageModel pattern.
    /// Removes System.Web, System.Web.UI, and System.Web.UI.WebControls dependencies
    /// which are not available in ASP.NET Core / Linux cloud environments.
    /// </summary>
    public class MainProfilePageModel : PageModel
    {
        /// <summary>
        /// Optional welcome message displayed on the profile page.
        /// Replaces the asp:Label (Label1) Web Forms server control.
        /// </summary>
        public string WelcomeMessage { get; private set; }

        /// <summary>
        /// Handles GET requests for the Main Profile Page.
        /// Replaces the Web Forms Page_Load event handler.
        /// </summary>
        public void OnGet()
        {
            // Business logic preserved from original Page_Load.
            // WelcomeMessage can be populated from session or query string as needed.
            WelcomeMessage = string.Empty;
        }
    }
}
