// MIGRATED: This Web Forms code-behind has been migrated to ASP.NET Core Razor Pages.
// The equivalent Razor Page model is located at: Pages/UserLogin.cshtml.cs
// This file is retained for reference only and is no longer active.
//
// Migration: ASP.NET Web Forms -> ASP.NET Core Razor Pages (cr-dotnet-0026)
// Rule: Web Forms Usage — Severity: HIGH — Category: legacy-framework-issues
//
// Changes applied (source: userlogin.aspx.cs, Lines 14-14):
//   Line 7:  Removed 'using System.Web.UI;'             -> No longer needed in Razor Pages
//   Line 8:  Removed 'using System.Web.UI.WebControls;' -> No longer needed in Razor Pages
//   Line 12: Removed 'using System.Web;'                -> No longer needed in Razor Pages
//   Line 14: Removed inheritance from System.Web.UI.Page -> PageModel base class used instead
//            'public partial class userlogin : System.Web.UI.Page'
//            replaced with 'public class UserLoginModel : PageModel' in Pages/UserLogin.cshtml.cs
//   Line 15: Removed Page_Load(object sender, EventArgs e) event handler
//            -> Replaced with OnGet() in Pages/UserLogin.cshtml.cs
//   Line 20: Removed Btn_Submit(object sender, EventArgs e) event handler
//            -> Replaced with OnPostLoginAsync() in Pages/UserLogin.cshtml.cs
//   Line 47: Removed Btn_reg(object sender, EventArgs e) event handler
//            -> Replaced with OnPostRegisterAsync() in Pages/UserLogin.cshtml.cs
//   Removed: Response.Redirect("MainProfilePage.aspx")
//            -> Replaced with RedirectToPage("/MainProfilePage") in Pages/UserLogin.cshtml.cs
//   Removed: Response.Redirect("SignUpForm.aspx")
//            -> Replaced with RedirectToPage("/SignUpForm") in Pages/UserLogin.cshtml.cs
//   Removed: Server.Transfer("MainProfilePage.aspx") / Server.Transfer("SignUpForm.aspx")
//            -> Not applicable in ASP.NET Core; RedirectToPage() used instead
//   Removed: SqlConnection with ConfigurationManager.ConnectionStrings
//            -> Replaced with IConfiguration + environment variable DB_CONNECTION_STRING
//   Removed: txtEmail.Text / txtPassword.Text server control property access
//            -> Replaced with [BindProperty] Input.Email / Input.Password model binding
//
// cr-dotnet-0010: Web.config Transformation Remediation
//   The original code at line 25 used ConfigurationManager.ConnectionStrings to read
//   the database connection string from Web.config, which was environment-specific
//   configuration baked into the build artifact via Web.Debug.config / Web.Release.config
//   XDT transformations.
//
//   This has been replaced in Pages/UserLogin.cshtml.cs with:
//     private string GetConnectionString()
//     {
//         return Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
//             ?? _configuration.GetConnectionString("dbconnection");
//     }
//
//   Configuration is now injected at runtime via:
//     - DB_CONNECTION_STRING environment variable (AWS ECS task definition,
//       Elastic Beanstalk environment properties, or AWS Systems Manager Parameter Store)
//   Web.Debug.config and Web.Release.config XDT transforms are no longer used.
//
// See Pages/UserLogin.cshtml.cs for the active Razor Page implementation.
// See Pages/UserLogin.cshtml for the active Razor Page view.
