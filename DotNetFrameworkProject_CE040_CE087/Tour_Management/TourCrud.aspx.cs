// MIGRATED: This Web Forms code-behind has been migrated to ASP.NET Core Razor Pages.
// The equivalent Razor Page model is located at: Pages/TourCrud.cshtml.cs
// This file is retained for reference only and is no longer active.
//
// Migration: ASP.NET Web Forms -> ASP.NET Core Razor Pages (cr-dotnet-0026)
// Changes applied:
//   Line 5:  Removed 'using System.Web.UI;'             -> No longer needed in Razor Pages
//   Line 6:  Removed 'using System.Web.UI.WebControls;' -> No longer needed in Razor Pages
//   Line 13: Removed inheritance from System.Web.UI.Page -> PageModel base class used instead
//   Line 15: Replaced Page_Load / IsPostBack pattern    -> OnGet() method in PageModel
//   Line 18: Replaced refreshdata() with GridView binding -> OnGet() loads data into Tours list
//
// cr-dotnet-0010: Web.config Transformation Remediation
//   The original code at line 25 used ConfigurationManager.ConnectionStrings to read
//   the database connection string from Web.config, which was environment-specific
//   configuration baked into the build artifact via Web.Debug.config / Web.Release.config
//   XDT transformations.
//
//   This has been replaced in Pages/TourCrud.cshtml.cs with:
//     private string GetConnectionString()
//     {
//         return Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
//             ?? _configuration.GetConnectionString("dbconnection");
//     }
//
//   Configuration is now injected at runtime via:
//     - DB_CONNECTION_STRING environment variable (AWS ECS task definition,
//       Elastic Beanstalk environment properties, or AWS Systems Manager Parameter Store)
//   Web.Debug.config and Web.Release.config XDT transforms are no longer used.
//
// See Pages/TourCrud.cshtml.cs for the active Razor Page implementation.
