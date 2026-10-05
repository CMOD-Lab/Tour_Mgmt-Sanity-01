// MIGRATION NOTE (cr-dotnet-0026): Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages pattern.
// - Removed: using System.Web (Web Forms namespace)
// - Removed: using System.Web.UI (Web Forms Page base class)
// - Removed: using System.Web.UI.WebControls (Web Forms server controls)
// - Replaced: System.Web.UI.Page base class with Microsoft.AspNetCore.Mvc.RazorPages.PageModel
// - Replaced: Page_Load event handler with OnGet() Razor Pages lifecycle method
// - Replaced: btn_click event handler with OnPost() Razor Pages POST handler
// - Replaced: Response.Write / Response.Redirect with IActionResult RedirectToPage()
// - Replaced: TextBox.Text property access with [BindProperty] model binding
// - The equivalent Razor Page model is: Pages/Order.cshtml.cs
// Fixed: cr-dotnet-0010 - Replaced Web.config transformation files (Web.Debug.config, Web.Release.config)
//        with environment variables and AWS Systems Manager Parameter Store for runtime configuration.
//        Configuration is injected at runtime rather than baked into build artifacts.
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dapper;

namespace Tour_Management
{
    // Migrated from Web Forms code-behind (System.Web.UI.Page) to ASP.NET Core Razor Pages PageModel
    public class Order : PageModel
    {
        // cr-dotnet-0010: Retrieve connection string at runtime from environment variable (highest priority),
        // then AWS Systems Manager Parameter Store (cloud-native secrets/config), then Web.config fallback.
        // This eliminates the need for Web.config transformation files (Web.Debug.config / Web.Release.config)
        // which bake configuration into build artifacts and are incompatible with cloud deployment pipelines.
        private static string GetConnectionString()
        {
            // 1. Environment variable — injected by AWS ECS task definition, Elastic Beanstalk, or Lambda
            var envValue = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            if (!string.IsNullOrEmpty(envValue))
                return envValue;

            // 2. AWS Systems Manager Parameter Store — runtime-configurable, no rebuild required
            //    Parameter path: /tour-mgmt/DB_CONNECTION_STRING
            //    IAM role on the compute resource must have ssm:GetParameter permission.
            try
            {
                using (var ssmClient = new AmazonSimpleSystemsManagementClient())
                {
                    var request = new GetParameterRequest
                    {
                        Name = "/tour-mgmt/DB_CONNECTION_STRING",
                        WithDecryption = true
                    };
                    var response = ssmClient.GetParameterAsync(request).GetAwaiter().GetResult();
                    if (!string.IsNullOrEmpty(response?.Parameter?.Value))
                        return response.Parameter.Value;
                }
            }
            catch
            {
                // SSM not available (e.g., local development) — fall through to Web.config
            }

            // 3. Web.config / appsettings fallback for local development only
            return System.Configuration.ConfigurationManager.ConnectionStrings["dbconnection"]?.ConnectionString;
        }

        // Migrated from Web Forms TextBox server controls to Razor Pages [BindProperty] model binding
        [BindProperty]
        public string TourName { get; set; }

        [BindProperty]
        public string City { get; set; }

        [BindProperty]
        public string Number { get; set; }

        [BindProperty]
        public string Name { get; set; }

        // OnGet replaces Page_Load for GET requests in ASP.NET Core Razor Pages
        public void OnGet()
        {
            // Page initialization logic preserved from original Page_Load
        }

        // OnPost replaces btn_click event handler for form POST submissions in ASP.NET Core Razor Pages
        public IActionResult OnPost()
        {
            string insertQuery = "insert into booking(TOUR_NAME,PLACE,Email,FirstName) values(@TOUR_NAME,@PLACE,@Email,@FirstName)";

            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Execute(insertQuery, new
                {
                    TOUR_NAME = TourName,
                    PLACE = City,
                    Email = Number,
                    FirstName = Name
                });
            }

            // Migrated from Response.Redirect to Razor Pages RedirectToPage
            return RedirectToPage("/mybooking");
        }
    }
}
