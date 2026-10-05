// MIGRATION NOTE (cr-dotnet-0026): ASP.NET Core Razor Pages PageModel for UserLogin page.
// Migrated from ASP.NET Web Forms (userlogin.aspx / userlogin.aspx.cs) to ASP.NET Core Razor Pages.
// - Removed: System.Web (line 7 - Web Forms namespace - cr-dotnet-0026)
// - Removed: System.Web.UI (line 8 - Web Forms namespace - cr-dotnet-0026)
// - Removed: System.Web.UI.WebControls (line 8 - Web Forms namespace - cr-dotnet-0026)
// - Removed: System.Web.UI.Page base class (line 12 - cr-dotnet-0026)
// - Replaced: System.Web.UI.Page base class with PageModel
// - Replaced: Page_Load event with OnGet() Razor Pages lifecycle method
// - Replaced: Btn_Submit event handler with OnPostLogin() Razor Pages POST handler
// - Replaced: Btn_reg event handler with OnPostRegister() Razor Pages POST handler
// - Replaced: TextBox.Text property access with [BindProperty] model binding
// - Replaced: Response.Redirect with RedirectToPage()
// - Replaced: SqlConnection + SqlCommand with Dapper parameterized query (SQL injection fix)
// - Fixed: cr-dotnet-0010 - Replaced Web.config transformation files (Web.Debug.config, Web.Release.config)
//          with environment variables and AWS Systems Manager Parameter Store for runtime configuration.
//          Configuration is injected at runtime rather than baked into build artifacts.
using System;
using System.Data;
using System.Data.SqlClient;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dapper;

namespace Tour_Management.Pages
{
    /// <summary>
    /// Razor Pages PageModel for the User Login page.
    /// Migrated from ASP.NET Web Forms userlogin code-behind.
    /// </summary>
    public class UserLoginModel : PageModel
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
                // SSM not available (e.g., local development) — fall through to appsettings.json
            }

            // 3. appsettings.json / Web.config fallback for local development only
            return System.Configuration.ConfigurationManager.ConnectionStrings["dbconnection"]?.ConnectionString;
        }

        // Migrated from Web Forms TextBox server controls to Razor Pages [BindProperty] model binding
        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        // OnGet replaces Page_Load for HTTP GET requests in ASP.NET Core Razor Pages
        public void OnGet()
        {
            // Page initialization logic preserved from original Page_Load
        }

        // OnPostLogin replaces Btn_Submit event handler for login form POST submissions
        // Migrated from: SqlConnection + SqlCommand with string concatenation (SQL injection risk)
        // Migrated to: Dapper parameterized query (secure)
        public IActionResult OnPostLogin()
        {
            string checkPasswordQuery = "SELECT password FROM Userinfo WHERE password=@Password AND email=@Email";
            string password;

            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                password = conn.QueryFirstOrDefault<string>(checkPasswordQuery, new
                {
                    Password = Password,
                    Email = Email
                }) ?? "";
            }

            if (password == Password)
            {
                // Migrated from Response.Redirect("MainProfilePage.aspx") to Razor Pages RedirectToPage
                return RedirectToPage("/MainProfilePage");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Password is not correct");
                return Page();
            }
        }

        // OnPostRegister replaces Btn_reg event handler for sign-up navigation
        // Migrated from: Response.Redirect("SignUpForm.aspx") to Razor Pages RedirectToPage
        public IActionResult OnPostRegister()
        {
            return RedirectToPage("/SignUpForm");
        }
    }
}
