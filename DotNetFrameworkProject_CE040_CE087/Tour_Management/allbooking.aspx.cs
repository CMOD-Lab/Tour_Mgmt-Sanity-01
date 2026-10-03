// MIGRATED TO ASP.NET CORE RAZOR PAGES
// This Web Forms code-behind file (allbooking.aspx.cs) has been migrated to
// ASP.NET Core Razor Pages. The new implementation is in allbooking.cshtml.cs.
//
// Migration performed as part of cloud readiness remediation (Rule: cr-dotnet-0026).
// The following Web Forms patterns have been removed and replaced:
//   - Line 5:  using System.Web;                  → removed (Web Forms dependency)
//   - Line 6:  using System.Web.UI;               → removed (Web Forms dependency)
//   - Line 10: public partial class allbooking : System.Web.UI.Page
//                                                 → replaced with PageModel (Razor Pages)
//   - Line 12: protected void Page_Load(...)      → replaced with OnGet() in Razor Pages
//
// The booking data retrieval logic (SELECT * FROM [booking]) has been preserved
// in allbooking.cshtml.cs as the LoadBookings() private method, using Dapper
// with Amazon RDS Proxy for cloud-native connection pooling.
