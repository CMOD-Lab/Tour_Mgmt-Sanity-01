// MIGRATED: This Web Forms code-behind has been migrated to ASP.NET Core Razor Pages.
// The equivalent Razor Page model is located at Pages/UserCrud.cshtml.cs.
// This file is retained for reference only and is no longer active.
// See Pages/UserCrud.cshtml.cs for the migrated PageModel implementation.

// Original Web Forms using statements replaced with ASP.NET Core equivalents:
// using System.Web;              -> removed (line 4 original)
// using System.Web.UI;           -> Microsoft.AspNetCore.Mvc.RazorPages  (line 5 original)
// using System.Web.UI.WebControls -> Microsoft.AspNetCore.Mvc  (line 6 original)

// Original class at line 10: public partial class usercrud : System.Web.UI.Page
// Migrated class: public class UserCrudModel : PageModel  (see Pages/UserCrud.cshtml.cs)
//
// Rule cr-dotnet-0026 remediation applied at lines 5, 6, 10, 12:
//   System.Web / System.Web.UI / System.Web.UI.WebControls usings removed.
//   System.Web.UI.Page inheritance removed; class migrated to ASP.NET Core Razor Pages PageModel.
//   asp:GridView with asp:SqlDataSource replaced by model-bound HTML table in UserCrud.cshtml.
//   SqlDataSource SelectCommand migrated to Dapper query in OnGet() in Pages/UserCrud.cshtml.cs.
//   SqlDataSource UpdateCommand migrated to OnPostUpdate() handler in Pages/UserCrud.cshtml.cs.
