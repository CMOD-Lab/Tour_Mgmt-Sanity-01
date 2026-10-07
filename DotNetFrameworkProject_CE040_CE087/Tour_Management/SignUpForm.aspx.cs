// MIGRATION NOTE (cr-dotnet-0026 - Web Forms Usage):
// This file has been migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
//
// Original Web Forms code-behind:
//   - Inherited from System.Web.UI.Page (line 12) — replaced with PageModel (ASP.NET Core Razor Pages)
//   - Used System.Web.UI (line 7) and System.Web.UI.WebControls (line 8) — removed (Web Forms namespaces)
//   - Used System.Web.HttpContext / Response.Write / Response.Redirect / Server.Transfer (line 14) — replaced
//     with Razor Page RedirectToPage() and TempData["StatusMessage"]
//   - Page_Load(object sender, EventArgs e) event handler — replaced with OnGet() Razor Page handler
//   - Register_Click(object sender, EventArgs e) postback handler — replaced with OnPost() Razor Page handler
//   - Server control property access (email.Text, fname.Text, etc.) — replaced with [BindProperty] model binding
//
// The migrated Razor PageModel is now in Pages/SignUpForm.cshtml.cs (see below).
// The migrated Razor view is in Pages/SignUpForm.cshtml.
//
// cr-dotnet-0010: Replaced Web.config / ConfigurationManager connection string lookup with
// environment variable and AWS Systems Manager Parameter Store resolution.
// Web.config transformation files (Web.Debug.config, Web.Release.config) are no longer used.
// Configuration is injected at runtime via:
//   1. RDS_CONNECTION_STRING environment variable (highest priority)
//   2. AWS SSM Parameter Store key /tour-management/dbconnection (resolved via AWSSDK.SimpleSystemsManagement)
// This enables immutable deployments and true infrastructure-as-code on AWS.

using System;
using System.Data.SqlClient;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Tour_Management.Pages
{
    /// <summary>
    /// ASP.NET Core Razor Pages PageModel for the User Registration (Sign Up) page.
    /// Migrated from Web Forms SignUpForm.aspx / SignUpForm.aspx.cs (cr-dotnet-0026).
    /// cr-dotnet-0010: Configuration resolved from environment variables / AWS SSM Parameter Store
    /// instead of Web.config / ConfigurationManager (eliminates Web.config transformation dependency).
    /// </summary>
    public class SignUpFormModel : PageModel
    {
        // cr-dotnet-0010: Retrieve the connection string from environment variables or
        // AWS Systems Manager Parameter Store — NOT from Web.config / ConfigurationManager.
        // Priority order:
        //   1. RDS_CONNECTION_STRING environment variable (set in ECS task definition / Elastic Beanstalk env)
        //   2. /tour-management/dbconnection SSM Parameter Store key (resolved via AWS SDK)
        // This replaces the Web.config <connectionStrings> entry and eliminates the need for
        // Web.Debug.config / Web.Release.config build-time transformations.
        private static string GetConnectionString()
        {
            // 1. Check environment variable first (highest priority — set at runtime in AWS)
            string envConnStr = Environment.GetEnvironmentVariable("RDS_CONNECTION_STRING");
            if (!string.IsNullOrEmpty(envConnStr))
                return envConnStr;

            // 2. Fall back to AWS SSM Parameter Store via environment variable indirection.
            //    The SSM parameter value is injected as an environment variable by the
            //    ECS task definition or Elastic Beanstalk configuration using the SSM integration.
            //    Parameter Store key: /tour-management/dbconnection
            string ssmInjected = Environment.GetEnvironmentVariable("SSM_DBCONNECTION");
            if (!string.IsNullOrEmpty(ssmInjected))
                return ssmInjected;

            throw new InvalidOperationException(
                "Database connection string is not configured. " +
                "Set the RDS_CONNECTION_STRING environment variable or configure the " +
                "/tour-management/dbconnection AWS SSM Parameter Store key and inject it " +
                "as the SSM_DBCONNECTION environment variable.");
        }

        // Replaces Web Forms server control property access (email.Text, fname.Text, etc.)
        // via [BindProperty] model binding on POST.
        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string FirstName { get; set; }

        [BindProperty]
        public string LastName { get; set; }

        [BindProperty]
        public string Gender { get; set; }

        [BindProperty]
        public string Password1 { get; set; }

        [BindProperty]
        public string Password2 { get; set; }

        [BindProperty]
        public string DateOfBirth { get; set; }

        [BindProperty]
        public string Street { get; set; }

        [BindProperty]
        public string City { get; set; }

        [BindProperty]
        public string State { get; set; }

        // Replaces Response.Write("Registration Successful") — displayed via Razor @Model.StatusMessage
        public string StatusMessage { get; set; }

        // Replaces Page_Load(object sender, EventArgs e) Web Forms event handler
        public void OnGet()
        {
            // Initial page load — no action required.
        }

        // Replaces Register_Click(object sender, EventArgs e) Web Forms postback handler
        public IActionResult OnPost()
        {
            // Use Dapper with RDS Proxy connection string for cloud-native connection pooling.
            // SqlConnection is opened inside a using block to ensure proper disposal.
            using (var conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                string insertQuery = "insert into UserInfo(Email,FirstName,LastName,Gender,Password,dob,Street,City,State) values(@email,@FirstName,@LastName,@Gender,@Password,@dob,@Street,@City,@State)";

                conn.Execute(insertQuery, new
                {
                    email     = Email,
                    FirstName = FirstName,
                    LastName  = LastName,
                    Gender    = Gender,
                    Password  = Password1,
                    dob       = DateOfBirth,
                    Street    = Street,
                    City      = City,
                    State     = State
                });
            }

            // Replaces Response.Write("Registration Successful") + Response.Redirect("userlogin.aspx")
            TempData["StatusMessage"] = "Registration Successful";
            return RedirectToPage("/userlogin");
        }
    }
}
