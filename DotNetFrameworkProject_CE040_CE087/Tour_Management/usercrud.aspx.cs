// MIGRATED TO ASP.NET CORE RAZOR PAGES
// This Web Forms code-behind (usercrud.aspx.cs) has been migrated to ASP.NET Core Razor Pages.
// The new implementation is located at: usercrud.cshtml / usercrud.cshtml.cs
//
// Migration performed as part of cloud readiness remediation (Rule: cr-dotnet-0026).
// The following Web Forms using statements have been removed:
//   - using System.Web;                    (line 5 - Web Forms HTTP context)
//   - using System.Web.UI;                 (line 6 - Web Forms Page base class)
//   - using System.Web.UI.WebControls;     (line 10 - Web Forms server controls)
// The System.Web.UI.Page inheritance (line 12) has been replaced with PageModel.
//
// Original code-behind class: Tour_Management.usercrud : System.Web.UI.Page
// Replacement: Tour_Management.Pages.UserCrudModel : PageModel (see usercrud.cshtml.cs)

using System;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Tour_Management.Pages
{
    // Retained as a stub to satisfy any remaining project references.
    // All active logic has been moved to usercrud.cshtml.cs (UserCrudModel).
    [Obsolete("Migrated to ASP.NET Core Razor Pages. See usercrud.cshtml / usercrud.cshtml.cs")]
    public class usercrud_WebFormsStub : PageModel
    {
        public void OnGet() { }
    }
}
