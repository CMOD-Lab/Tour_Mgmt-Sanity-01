using System;
using System.Web.UI;

namespace Tour_Management
{
    /// <summary>
    /// usercrud.aspx code-behind - MIGRATED TO ASP.NET MVC
    /// Rule: cr-dotnet-0026 - Web Forms Usage
    ///
    /// This Web Form has been replaced by:
    ///   - Controller: UserController.UserCrud (GET) in Controllers/UserController.cs
    ///   - View: Views/User/UserCrud.cshtml
    ///   - Model: Models/UserInfoViewModel (in Models/TourModels.cs)
    ///
    /// The original Web Forms patterns removed:
    ///   - System.Web.UI.Page inheritance (line 10) → replaced by Controller : Controller
    ///   - System.Web.UI / System.Web.UI.WebControls using statements (lines 5-6) → removed
    ///   - asp:GridView with asp:SqlDataSource controls (lines 10, 12) → replaced by Razor table
    ///   - Page_Load event handler → replaced by [HttpGet] ActionResult
    ///   - SqlDataSource SelectCommand/UpdateCommand → replaced by Dapper queries in controller
    /// </summary>
    public partial class usercrud : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Redirect to the ASP.NET MVC equivalent route
            Response.RedirectPermanent("~/User/UserCrud");
        }
    }
}
