// MIGRATION NOTICE: This Web Forms code-behind (AddTour.aspx.cs) has been migrated to
// ASP.NET Core Razor Pages. The equivalent page model is AddTour.cshtml.cs.
//
// Rule cr-dotnet-0026: Web Forms Usage - Migrated to ASP.NET Core Razor Pages
// for cloud-native deployment, improved performance, and horizontal scalability.
//
// Rule cr-dotnet-1032: Server.MapPath for File Access in Cloud - Replaced with
// Amazon S3 file operations using the AWS SDK for .NET. The original line 33:
//   FileUpload1.SaveAs(Server.MapPath("~/Tour_pics/") + FileUpload1.FileName);
// has been replaced with S3 PutObjectRequest upload in AddTour.cshtml.cs.
//
// Rule cr-dotnet-0010: Web.config Transformations - Replaced with environment variables
// and AWS Systems Manager Parameter Store. Configuration is now injected at runtime:
//   - DB_CONNECTION_STRING environment variable replaces Web.config connectionStrings
//   - AWS SSM Parameter Store (/tour-management/db-connection-string) for secrets
//   - Web.Debug.config and Web.Release.config build-time transforms are eliminated
//
// The following Web Forms-specific namespaces and patterns have been replaced:
//   - System.Web.UI (line 7) → Microsoft.AspNetCore.Mvc.RazorPages
//   - System.Web.UI.WebControls (line 8) → Microsoft.AspNetCore.Mvc / BindProperty attributes
//   - System.Web.UI.Page inheritance (line 12) → PageModel base class
//   - Page_Load event handler (line 14) → OnGet() method
//   - ConfigurationManager.ConnectionStrings["dbconnection"] (line 21) →
//       Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
//   - Server.MapPath("~/Tour_pics/") (line 33) → Amazon S3 PutObjectRequest (AWSSDK.S3)
//
// See AddTour.cshtml.cs for the migrated implementation.

using System;
using System.Data;
using System.Data.SqlClient;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Dapper;

namespace Tour_Management.Pages
{
    // Migrated from Web Forms AddTour : System.Web.UI.Page
    // to ASP.NET Core Razor Pages AddTourModel : PageModel
    //
    // cr-dotnet-0010 remediation applied:
    //   Connection string is now read from the DB_CONNECTION_STRING environment variable
    //   (or AWS Systems Manager Parameter Store) at runtime, replacing the Web.config
    //   <connectionStrings> element and its Debug/Release XDT transformation files.
    //
    //   Example runtime injection (ECS task definition / EC2 environment):
    //     DB_CONNECTION_STRING=Server=<rds-proxy-endpoint>;Database=tourdb;User Id=<user>;Password=<secret>;
    //
    //   AWS SSM Parameter Store path (recommended for secrets):
    //     /tour-management/db-connection-string
    //
    // File upload migrated from Server.MapPath (line 33) to Amazon S3 (AWSSDK.S3).
    // See AddTour.cshtml.cs for the full implementation.
}
