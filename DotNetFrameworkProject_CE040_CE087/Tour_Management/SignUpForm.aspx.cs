// MIGRATED TO ASP.NET CORE RAZOR PAGES
// This Web Forms code-behind file (SignUpForm.aspx.cs) has been migrated to
// ASP.NET Core Razor Pages. The new implementation is in SignUpForm.cshtml.cs.
//
// Migration performed as part of cloud readiness remediation (Rule: cr-dotnet-0026).
// The following Web Forms patterns have been removed and replaced:
//   - Line 7:  using System.Web;                  → removed (Web Forms dependency)
//   - Line 8:  using System.Web.UI;               → removed (Web Forms dependency)
//   - Line 12: using System.Web.UI.WebControls;   → removed (Web Forms dependency)
//   - Line 14: System.Web.UI.Page inheritance     → replaced with PageModel (Razor Pages)
//
// Rule cr-dotnet-0010: Web.config Transformations - Replaced with environment variables
// and AWS Systems Manager Parameter Store. Configuration is now injected at runtime:
//   - Line 21: ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString →
//              Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
//   - AWS SSM Parameter Store path: /tour-management/db-connection-string
//   - Web.Debug.config and Web.Release.config build-time transforms are eliminated;
//     environment-specific configuration is supplied via environment variables at runtime.
//
// The Register_Click event handler logic has been preserved in SignUpForm.cshtml.cs
// as the OnPostRegister() handler, using Dapper with Amazon RDS Proxy for
// cloud-native connection pooling.
