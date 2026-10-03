// ============================================================
// MIGRATED TO ASP.NET CORE RAZOR PAGES
// Rule: cr-dotnet-0026 — Web Forms Usage
// ============================================================
// This Web Forms code-behind (userlogin.aspx.cs) has been fully
// migrated to ASP.NET Core Razor Pages for cloud-native deployment,
// improved performance, and horizontal scalability on AWS.
//
// The new implementation is located at:
//   - View  : userlogin.cshtml
//   - Model : userlogin.cshtml.cs  (UserLoginModel : PageModel)
//
// Web Forms patterns removed (original lines 7–14):
//   REMOVED: using System.Web;                  — Web Forms HTTP context
//   REMOVED: using System.Web.UI;               — Web Forms Page base class
//   REMOVED: using System.Web.UI.WebControls;   — Web Forms server controls
//   REMOVED: public partial class userlogin : System.Web.UI.Page  (line 14)
//            → Replaced with: UserLoginModel : PageModel
//
// Rule cr-dotnet-0010: Web.config Transformations - Replaced with environment variables
// and AWS Systems Manager Parameter Store. Configuration is now injected at runtime:
//   - Line 25: ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString →
//              Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
//   - AWS SSM Parameter Store path: /tour-management/db-connection-string
//   - Web.Debug.config and Web.Release.config build-time transforms are eliminated;
//     environment-specific configuration is supplied via environment variables at runtime.
//
// Web Forms runtime patterns replaced:
//   Response.Write(...)        → StatusMessage property rendered in Razor view
//   Response.Redirect(...)     → RedirectToPage() IActionResult
//   Server.Transfer(...)       → RedirectToPage() IActionResult
//   SqlConnection (direct)     → Dapper with Amazon RDS Proxy connection pooling
//   asp:TextBox controls       → HTML <input> elements with Razor model binding
//   asp:Button OnClick events  → Razor Pages handler methods (OnPostLogin, OnPostSignUp)
//   ConfigurationManager       → Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
// ============================================================

using System;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Tour_Management.Pages
{
    /// <summary>
    /// Compatibility stub retained to satisfy any remaining project references to
    /// the original Web Forms code-behind file (userlogin.aspx.cs).
    ///
    /// All active business logic has been moved to:
    ///   userlogin.cshtml.cs → UserLoginModel : PageModel
    ///
    /// This stub inherits PageModel (ASP.NET Core) instead of System.Web.UI.Page
    /// (ASP.NET Web Forms), completing the migration from Web Forms to Razor Pages.
    ///
    /// cr-dotnet-0010 remediation applied:
    ///   Connection string is now read from the DB_CONNECTION_STRING environment variable
    ///   (or AWS Systems Manager Parameter Store) at runtime, replacing the Web.config
    ///   <connectionStrings> element and its Debug/Release XDT transformation files.
    ///
    ///   Example runtime injection (ECS task definition / EC2 environment):
    ///     DB_CONNECTION_STRING=Server=<rds-proxy-endpoint>;Database=tourdb;User Id=<user>;Password=<secret>;
    ///
    ///   AWS SSM Parameter Store path (recommended for secrets):
    ///     /tour-management/db-connection-string
    /// </summary>
    [Obsolete("Migrated to ASP.NET Core Razor Pages. See userlogin.cshtml / userlogin.cshtml.cs")]
    public class userlogin_WebFormsStub : PageModel
    {
        /// <summary>
        /// No-op GET handler — all logic is in UserLoginModel (userlogin.cshtml.cs).
        /// </summary>
        public void OnGet() { }
    }
}
