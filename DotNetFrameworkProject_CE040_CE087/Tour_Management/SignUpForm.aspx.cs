using System;
using System.Web.UI;

namespace Tour_Management
{
    /// <summary>
    /// SignUpForm.aspx code-behind - MIGRATED TO ASP.NET MVC
    /// Rule: cr-dotnet-0026 - Web Forms Usage
    /// 
    /// This Web Form has been replaced by:
    ///   - Controller: UserController.SignUp (GET/POST) in Controllers/UserController.cs
    ///   - View: Views/User/SignUp.cshtml
    ///   - Model: Models/SignUpViewModel (in Models/TourModels.cs)
    ///
    /// The original Web Forms patterns removed:
    ///   - System.Web.UI.Page inheritance (line 12) → replaced by Controller : Controller
    ///   - System.Web.UI / System.Web.UI.WebControls using statements (lines 7-8) → removed
    ///   - Page_Load event handler (line 14) → replaced by [HttpGet] ActionResult
    ///   - Register_Click event handler → replaced by [HttpPost] ActionResult
    ///   - Response.Redirect("userlogin.aspx") → replaced by RedirectToAction("Login", "User")
    ///   - Server-side controls (asp:TextBox, asp:Label, etc.) → replaced by Razor HTML helpers
    ///
    /// cr-dotnet-0010 fix applied: ConfigurationManager.ConnectionStrings["dbconnection"]
    /// has been replaced with environment variable DB_CONNECTION_STRING (with fallback to
    /// AWS Systems Manager Parameter Store key /tour-management/db-connection-string).
    /// Web.config transformation files (Web.Debug.config, Web.Release.config) are no longer
    /// used for connection string injection — configuration is injected at runtime via
    /// environment variables, enabling immutable deployments on AWS.
    ///
    /// Original cloud-incompatible pattern (cr-dotnet-0010) removed:
    ///   SqlConnection conn = new SqlConnection(
    ///       ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);
    ///
    /// Replaced by (in UserController.cs):
    ///   string connStr = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
    ///       ?? AwsSsmHelper.GetParameter("/tour-management/db-connection-string");
    /// </summary>
    public partial class SignUpForm : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Redirect to the ASP.NET MVC equivalent route
            Response.RedirectPermanent("~/User/SignUp");
        }
    }
}
