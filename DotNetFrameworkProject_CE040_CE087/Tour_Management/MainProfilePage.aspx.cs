// MIGRATION NOTICE (Rule cr-dotnet-0026 — Web Forms Usage):
// This code-behind file has been migrated to ASP.NET Core Razor Pages.
// See MainProfilePage.cshtml.cs (Tour_Management.Pages.MainProfilePageModel).
//
// The following Web Forms-specific patterns have been replaced:
//   Line 5: "using System.Web;" — removed; System.Web is not available in ASP.NET Core / Linux.
//   Line 6: "using System.Web.UI;" — removed; replaced by Microsoft.AspNetCore.Mvc.RazorPages.
//   Line 10: "public partial class MainProfilePage : System.Web.UI.Page" — replaced by PageModel.
//   Line 12: "protected void Page_Load(object sender, EventArgs e)" — replaced by OnGet().
//
// The Razor Page equivalent is:
//   using Microsoft.AspNetCore.Mvc.RazorPages;
//   public class MainProfilePageModel : PageModel { public void OnGet() { } }

using System;
using System.Collections.Generic;
using System.Linq;
// Removed: using System.Web;               // Not available in ASP.NET Core (Linux cloud target)
// Removed: using System.Web.UI;            // Not available in ASP.NET Core (Linux cloud target)
// Removed: using System.Web.UI.WebControls; // Not available in ASP.NET Core (Linux cloud target)

namespace Tour_Management
{
    // Retained for reference only. Active implementation is in MainProfilePage.cshtml.cs.
    // public partial class MainProfilePage : System.Web.UI.Page  <-- Web Forms pattern removed
    // {
    //     protected void Page_Load(object sender, EventArgs e)   <-- Web Forms event removed
    //     {
    //     }
    // }
}
