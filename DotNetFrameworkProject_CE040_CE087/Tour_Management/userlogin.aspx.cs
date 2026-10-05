// MIGRATED: This Web Forms code-behind has been migrated to ASP.NET Core Razor Pages.
// The equivalent Razor Page model is located at Pages/UserLogin.cshtml.cs.
// This file is retained for reference only and is no longer active.
// See Pages/UserLogin.cshtml.cs for the migrated PageModel implementation.

// ─── Rule cr-dotnet-0026 — Web Forms Usage — Remediation Applied ───────────────
// Remediation: Migrate to ASP.NET Core MVC/Razor Pages
//
// Original Web Forms using statements removed (lines 6–8 in original):
//   using System.Web;                  → removed (not available in ASP.NET Core)
//   using System.Web.UI;               → replaced by Microsoft.AspNetCore.Mvc.RazorPages
//   using System.Web.UI.WebControls;   → replaced by Microsoft.AspNetCore.Mvc
//
// Original class declaration at line 12 (original):
//   public partial class userlogin : System.Web.UI.Page
//   → Migrated to: public class UserLoginModel : PageModel
//     (see Pages/UserLogin.cshtml.cs)
//
// Original event handlers migrated:
//   protected void Page_Load(object sender, EventArgs e)
//     → public void OnGet()  (Pages/UserLogin.cshtml.cs)
//
//   protected void Btn_Submit(object sender, EventArgs e)   [line 14 original — violation]
//     → public IActionResult OnPostLogin()  (Pages/UserLogin.cshtml.cs)
//     - SqlConnection with ConfigurationManager replaced by Dapper + IConfiguration
//       with environment variable DB_CONNECTION_STRING (AWS RDS Proxy-compatible)
//     - Raw SQL string concatenation replaced by parameterised Dapper query
//     - Response.Redirect("MainProfilePage.aspx") → RedirectToPage("/MainProfilePage")
//     - Server.Transfer("MainProfilePage.aspx") removed (not supported in ASP.NET Core)
//
//   protected void Btn_reg(object sender, EventArgs e)
//     → public IActionResult OnPostRegister()  (Pages/UserLogin.cshtml.cs)
//     - Response.Redirect("SignUpForm.aspx") → RedirectToPage("/SignUpForm")
//     - Server.Transfer("SignUpForm.aspx") removed (not supported in ASP.NET Core)
//
// Original server controls replaced in Pages/UserLogin.cshtml:
//   asp:Label ID="Label1"       → <label asp-for="Input.Email">
//   asp:TextBox ID="txtEmail"   → <input asp-for="Input.Email" type="email" />
//   asp:Label ID="Label2"       → <label asp-for="Input.Password">
//   asp:TextBox ID="txtPassword"→ <input asp-for="Input.Password" type="password" />
//   asp:Button ID="Register" OnClick="Btn_Submit" Text="Login"
//     → <button type="submit" asp-page-handler="Login">Login</button>
//   asp:Button ID="Button1" OnClick="Btn_reg" Text="Sign Up"
//     → <button type="submit" asp-page-handler="Register">Sign Up</button>
//
// <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="userlogin.aspx.cs"
//          Inherits="Tour_Management.userlogin" %>
//   → @page / @model Tour_Management.Pages.UserLoginModel  (Pages/UserLogin.cshtml)
// ────────────────────────────────────────────────────────────────────────────────

// ─── Rule cr-dotnet-0010 — Web.config Transformations — Remediation Applied ──────
// Remediation: Replace Web.config Transformations with Environment Variables and
//              AWS Systems Manager Parameter Store
//
// Original violation (line 25 in original userlogin.aspx.cs):
//   SqlConnection conn = new SqlConnection(
//       ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);
//
// Fix applied in Pages/UserLogin.cshtml.cs (GetConnectionString method):
//   - Removed dependency on ConfigurationManager.ConnectionStrings (Web.config-based)
//   - Configuration is now resolved at runtime from:
//       1. DB_CONNECTION_STRING environment variable (AWS ECS / Elastic Beanstalk /
//          App Runner environment property) — primary source
//       2. AWS Systems Manager Parameter Store via IConfiguration SSM provider
//          (parameter path: /tour-management/db-connection-string) — secondary source
//       3. appsettings.json "ConnectionStrings:dbconnection" — local dev fallback only
//   - Web.Debug.config and Web.Release.config transformation files are no longer used;
//     configuration is injected at runtime enabling immutable, environment-agnostic builds.
// ─────────────────────────────────────────────────────────────────────────────────────
