// userlogin.aspx.cs - MIGRATED TO ASP.NET MVC
// Rule: cr-dotnet-0026 - Web Forms Usage
//
// This Web Forms code-behind has been fully migrated to ASP.NET MVC.
// The System.Web.UI.Page inheritance and all Web Forms patterns have been removed.
//
// Replacement components:
//   - Controller : Controllers/UserController.cs
//       GET  /User/Login  → UserController.Login()
//       POST /User/Login  → UserController.Login(UserLoginViewModel model)
//   - Razor View : Views/User/Login.cshtml
//   - View Model : Models/TourModels.cs → UserLoginViewModel
//
// Migration summary (original Web Forms patterns removed):
//   - Removed: using System.Web.UI;                          (was line 7)
//   - Removed: using System.Web.UI.WebControls;              (was line 8)
//   - Removed: public partial class userlogin : System.Web.UI.Page  (was line 12)
//   - Removed: Page_Load(object sender, EventArgs e)         (was line 14)
//   - Removed: Btn_Submit event handler with SqlConnection   (was lines 19-50)
//   - Removed: Response.Redirect("MainProfilePage.aspx")     → replaced by RedirectToAction("MainProfilePage", "Home")
//   - Removed: Server.Transfer("MainProfilePage.aspx")       → not needed in MVC
//   - Removed: Btn_reg event handler                         → replaced by RedirectToAction("SignUp", "User")
//   - Removed: txtEmail.Text / txtPassword.Text server controls → replaced by UserLoginViewModel properties
//
// cr-dotnet-0010 fix applied: ConfigurationManager.ConnectionStrings["dbconnection"]
// has been replaced with environment variable DB_CONNECTION_STRING (with fallback to
// AWS Systems Manager Parameter Store key /tour-management/db-connection-string).
// Web.config transformation files (Web.Debug.config, Web.Release.config) are no longer
// used for connection string injection — configuration is injected at runtime via
// environment variables, enabling immutable deployments on AWS.
//
// Original cloud-incompatible pattern (cr-dotnet-0010) removed:
//   SqlConnection conn = new SqlConnection(
//       ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);
//
// Replaced by (in UserController.cs):
//   string connStr = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
//       ?? AwsSsmHelper.GetParameter("/tour-management/db-connection-string");
//
// All business logic has been preserved in UserController.cs:
//   - Password validation query: SELECT password FROM Userinfo WHERE password=@Password AND email=@Email
//   - Successful login redirects to MainProfilePage (Home controller)
//   - Failed login returns validation error message
//   - Registration navigation redirects to SignUp action

namespace Tour_Management
{
    // This file is intentionally left as a migration marker.
    // The userlogin Web Form has been fully replaced by the MVC pattern.
    // No System.Web.UI.Page inheritance remains — the cloud readiness blocker is resolved.
}
