// MIGRATION NOTE (cr-dotnet-0026 - Web Forms Usage):
// This file has been migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
//
// Original Web Forms code-behind patterns removed:
//   - Used System.Web (line 5) — removed (Web Forms namespace, not available in ASP.NET Core)
//   - Used System.Web.UI (line 6) — removed (Web Forms namespace, not available in ASP.NET Core)
//   - Used System.Web.UI.WebControls (line 7) — removed (Web Forms namespace, not available in ASP.NET Core)
//   - Inherited from System.Web.UI.Page (line 12) — replaced with PageModel (ASP.NET Core Razor Pages)
//   - Page_Load(object sender, EventArgs e) event handler — replaced with OnGet() Razor Page handler
//   - Btn_Submit(object sender, EventArgs e) postback handler — replaced with OnPostLogin() Razor Page handler
//   - Btn_reg(object sender, EventArgs e) postback handler — replaced with OnPostRegister() Razor Page handler
//   - txtEmail.Text / txtPassword.Text server control access — replaced with [BindProperty] model binding
//   - Response.Redirect("MainProfilePage.aspx") — replaced with RedirectToPage("/MainProfilePage")
//   - Response.Redirect("SignUpForm.aspx") — replaced with RedirectToPage("/SignUpForm")
//   - Server.Transfer() — removed (not applicable in ASP.NET Core)
//
// The migrated Razor PageModel is now in Pages/UserLogin.cshtml.cs
// The migrated Razor view is in Pages/UserLogin.cshtml
//
// cr-dotnet-0010: Replaced Web.config / ConfigurationManager connection string lookup with
// environment variable and AWS Systems Manager Parameter Store resolution.
// Web.config transformation files (Web.Debug.config, Web.Release.config) are no longer used.
// Configuration is injected at runtime via:
//   1. RDS_CONNECTION_STRING environment variable (highest priority)
//   2. AWS SSM Parameter Store key /tour-management/dbconnection (injected as SSM_DBCONNECTION env var)
// This enables immutable deployments and true infrastructure-as-code on AWS.
