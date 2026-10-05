// MIGRATION NOTE (cr-dotnet-0026): This file has been fully migrated to ASP.NET Core Razor Pages.
// The Razor Pages equivalent is located at: Pages/UserLogin.cshtml and Pages/UserLogin.cshtml.cs
//
// Original Web Forms violations addressed (cr-dotnet-0026 - Web Forms Usage):
//   - Line 7:  using System.Web;                → removed (Web Forms namespace)
//   - Line 8:  using System.Web.UI;             → removed (Web Forms namespace)
//   - Line 9:  using System.Web.UI.WebControls; → removed (Web Forms namespace)
//   - Line 12: public partial class userlogin : System.Web.UI.Page
//              → replaced with UserLoginModel : PageModel in Pages/UserLogin.cshtml.cs
//   - Line 14: using System.Configuration;      → replaced with Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
//   - Line 16: using System.Data.SqlClient;     → replaced with Dapper parameterized queries in Pages/UserLogin.cshtml.cs
//
// Fixed: cr-dotnet-0010 - Replaced Web.config transformation files (Web.Debug.config, Web.Release.config)
//        with environment variables and AWS Systems Manager Parameter Store for runtime configuration.
//        Configuration is injected at runtime rather than baked into build artifacts, enabling
//        true infrastructure-as-code and immutable deployments.
//
// Remediation: Migrate to ASP.NET Core Razor Pages (cr-dotnet-0026)
//   - ASP.NET Web Forms Page lifecycle (Page_Load, event handlers) replaced with Razor Pages lifecycle
//   - System.Web.UI.Page base class replaced with Microsoft.AspNetCore.Mvc.RazorPages.PageModel
//   - Server controls (<asp:TextBox>, <asp:Button>, <asp:Label>) replaced with HTML tag helpers
//   - runat="server" attributes removed throughout
//   - <form runat="server"> replaced with <form method="post"> with AntiForgeryToken
//   - Response.Redirect() replaced with RedirectToPage() (Razor Pages navigation)
//   - Server.Transfer() removed (not supported in ASP.NET Core)
//   - ConfigurationManager.ConnectionStrings replaced with:
//       1. Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")  [highest priority]
//       2. AWS Systems Manager Parameter Store (/tour-mgmt/DB_CONNECTION_STRING)  [cloud-native]
//       3. appsettings.json fallback  [local development only]
//   - SqlConnection + SqlCommand with string concatenation replaced with Dapper parameterized queries
//   - [BindProperty] model binding replaces TextBox.Text server control access
//
// cr-dotnet-0010: Configuration Resolution Strategy (replaces Web.config transformations):
//   Environment variables and AWS SSM Parameter Store are used instead of Web.Debug.config /
//   Web.Release.config build-time transformations. This enables immutable deployments where
//   the same build artifact is promoted across environments (dev → staging → production)
//   with configuration injected at runtime by the AWS platform.
//
//   AWS SSM Parameter paths:
//     /tour-mgmt/DB_CONNECTION_STRING  (SecureString, encrypted at rest)
//     /tour-mgmt/S3_BUCKET_NAME
//     /tour-mgmt/AWS_REGION
//
//   Set via AWS CLI:
//     aws ssm put-parameter --name "/tour-mgmt/DB_CONNECTION_STRING" \
//       --value "<connection-string>" --type SecureString --overwrite
//
// Active implementation: Pages/UserLogin.cshtml (view) + Pages/UserLogin.cshtml.cs (PageModel)
//
// This file is retained as a migration reference only and is NOT compiled as part of the
// ASP.NET Core application. All business logic has been preserved in the Razor Pages implementation.
