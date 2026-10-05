// MIGRATED: This Web Forms code-behind has been migrated to ASP.NET Core Razor Pages.
// The equivalent Razor Page model is located at Pages/TourCrud.cshtml.cs.
// This file is retained for reference only and is no longer active.
// See Pages/TourCrud.cshtml.cs for the migrated PageModel implementation.

// Original Web Forms using statements replaced with ASP.NET Core equivalents:
// using System.Web.UI;           -> Microsoft.AspNetCore.Mvc.RazorPages  (line 18 original)
// using System.Web.UI.WebControls -> Microsoft.AspNetCore.Mvc

// Original class: public partial class TourCrud : System.Web.UI.Page  (line 18 original)
// Migrated class: public class TourCrudModel : PageModel  (see Pages/TourCrud.cshtml.cs)
//
// Rule cr-dotnet-0026 remediation applied at line 18:
//   System.Web.UI.Page inheritance removed; class migrated to ASP.NET Core Razor Pages PageModel.
//   refreshdata() method migrated to OnGet() + RefreshData() in Pages/TourCrud.cshtml.cs.
//   SqlConnection usage retained in Razor Page with environment-variable-based connection string.

// ─── Rule cr-dotnet-0010 — Web.config Transformations — Remediation Applied ──────
// Remediation: Replace Web.config Transformations with Environment Variables and
//              AWS Systems Manager Parameter Store
//
// Original violation (line 25 in original TourCrud.aspx.cs):
//   SqlConnection conn = new SqlConnection(
//       ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);
//
// Fix applied in Pages/TourCrud.cshtml.cs (GetConnectionString method):
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
