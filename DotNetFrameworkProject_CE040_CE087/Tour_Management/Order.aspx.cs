// MIGRATION NOTICE (Rule cr-dotnet-0026 — Web Forms Usage):
// This code-behind file has been migrated to ASP.NET Core Razor Pages.
// See Order.cshtml.cs (Tour_Management.Pages.OrderModel).
//
// Rule cr-dotnet-0010: Web.config Transformations - Replaced with environment variables
// and AWS Systems Manager Parameter Store. Configuration is now injected at runtime:
//   - DB_CONNECTION_STRING environment variable replaces Web.config connectionStrings
//   - AWS SSM Parameter Store (/tour-management/db-connection-string) for secrets
//   - Web.Debug.config and Web.Release.config build-time transforms are eliminated
//
// The following Web Forms-specific patterns have been replaced:
//   Line 7:  "using System.Web;"               — removed; System.Web is not available in ASP.NET Core / Linux.
//   Line 8:  "using System.Web.UI;"             — removed; replaced by Microsoft.AspNetCore.Mvc.RazorPages.
//   Line 12: "public partial class Order : System.Web.UI.Page" — replaced by PageModel.
//   Line 14: "protected void btn_click(object sender, EventArgs e)" — replaced by OnPostRegister().
//   Line 21: "ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString" —
//            replaced by Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
//
// The Razor Page equivalent is in Order.cshtml.cs:
//   using Microsoft.AspNetCore.Mvc.RazorPages;
//   public class OrderModel : PageModel
//   {
//       public IActionResult OnPostRegister() { ... }  // replaces btn_click event handler
//   }
//
// Additional cloud-readiness improvements in Order.cshtml.cs:
//   - Replaced direct SqlConnection / SqlCommand with Dapper for connection pooling
//   - Connection string sourced from DB_CONNECTION_STRING environment variable (12-factor)
//   - AWS SSM Parameter Store path: /tour-management/db-connection-string
//   - Replaced Response.Write / Response.Redirect with RedirectToPage (ASP.NET Core pattern)
//   - Removed Server.Transfer (Web Forms only, not available in ASP.NET Core)

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
// Removed: using System.Configuration;        // Replaced by Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
// Removed: using System.Web;                  // Not available in ASP.NET Core (Linux cloud target)
// Removed: using System.Web.UI;               // Not available in ASP.NET Core (Linux cloud target)
// Removed: using System.Web.UI.WebControls;   // Not available in ASP.NET Core (Linux cloud target)

namespace Tour_Management
{
    // Retained for reference only. Active implementation is in Order.cshtml.cs.
    // public partial class Order : System.Web.UI.Page  <-- Web Forms pattern removed
    // {
    //     protected void Page_Load(object sender, EventArgs e) { }
    //
    //     protected void btn_click(object sender, EventArgs e)  <-- Web Forms event removed
    //     {
    //         // cr-dotnet-0010: ConfigurationManager.ConnectionStrings removed.
    //         // Replaced with: Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
    //         // or AWS SSM Parameter Store: /tour-management/db-connection-string
    //         SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING"));
    //         conn.Open();
    //         string insertQuery = "insert into booking(TOUR_NAME,PLACE,Email,FirstName) values(@TOUR_NAME,@PLACE,@Email,@FirstName)";
    //         SqlCommand com = new SqlCommand(insertQuery, conn);
    //         com.Parameters.AddWithValue("@TOUR_NAME", tour_name.Text);
    //         com.Parameters.AddWithValue("@PLACE", city.Text);
    //         com.Parameters.AddWithValue("@Email", number.Text);
    //         com.Parameters.AddWithValue("@FirstName", name.Text);
    //         com.ExecuteNonQuery();
    //         Response.Write("Registration Successful");
    //         Response.Redirect("mybooking.aspx");
    //         Server.Transfer("mybooking.aspx");
    //         conn.Close();
    //     }
    // }
}
