// MIGRATION NOTICE: This Web Forms code-behind (AdminLogin2.aspx.cs) has been migrated to
// ASP.NET Core Razor Pages. The equivalent page model is AdminLogin2.cshtml.cs.
//
// Rule cr-dotnet-0026: Web Forms Usage - Migrated to ASP.NET Core Razor Pages
// for cloud-native deployment, improved performance, and horizontal scalability.
//
// The following Web Forms-specific namespaces and patterns have been replaced:
//   - System.Web (line 5) → Microsoft.AspNetCore.Mvc
//   - System.Web.UI (line 5) → Microsoft.AspNetCore.Mvc.RazorPages
//   - System.Web.UI.WebControls (line 6) → BindProperty attributes
//   - System.Web.UI.Page inheritance (line 10) → PageModel base class
//   - Page_Load event handler (line 12) → OnGet() / OnPostLogin() methods
//   - Response.Redirect / Server.Transfer → RedirectToPage()
//   - Hardcoded admin credentials → Environment variables (ADMIN_EMAIL, ADMIN_PASSWORD)
//
// See AdminLogin2.cshtml.cs for the migrated implementation.

using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace Tour_Management.Pages
{
    // Migrated from Web Forms AdminLogin2 : System.Web.UI.Page
    // to ASP.NET Core Razor Pages AdminLogin2Model : PageModel
    // See AdminLogin2.cshtml.cs for the full implementation.
}
