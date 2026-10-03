// MIGRATED TO ASP.NET CORE RAZOR PAGES
// This Web Forms code-behind file (TourCrud.aspx.cs) has been migrated to
// ASP.NET Core Razor Pages. The new implementation is in TourCrud.cshtml.cs.
//
// Migration performed as part of cloud readiness remediation (Rule: cr-dotnet-0026).
// The following Web Forms patterns have been removed and replaced:
//   - Line 5:  using System.Web;                  → removed (Web Forms dependency)
//   - Line 6:  using System.Web.UI;               → removed (Web Forms dependency)
//   - Line 13: using System.Web.UI.WebControls;   → removed (Web Forms dependency)
//   - Line 15: System.Web.UI.Page inheritance     → replaced with PageModel (Razor Pages)
//
// Rule cr-dotnet-0010: Web.config Transformations - Replaced with environment variables
// and AWS Systems Manager Parameter Store. Configuration is now injected at runtime:
//   - Line 25: ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString →
//              Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
//   - AWS SSM Parameter Store path: /tour-management/db-connection-string
//   - Web.Debug.config and Web.Release.config build-time transforms are eliminated;
//     environment-specific configuration is supplied via environment variables at runtime.
//
// The refreshdata() method logic has been preserved in TourCrud.cshtml.cs
// as the LoadTours() private method, using Dapper with Amazon RDS Proxy for
// cloud-native connection pooling. Full CRUD operations (Edit, Update, Delete)
// previously handled by asp:SqlDataSource are now implemented as Razor Page
// handler methods: OnPostEdit(), OnPostUpdate(), OnPostDelete().
