// MIGRATION NOTE (cr-dotnet-0026 - Web Forms Usage):
// This file is the ASP.NET Core Razor Pages code-behind for the User Login page.
// Migrated from ASP.NET Web Forms (userlogin.aspx / userlogin.aspx.cs).
//
// Original Web Forms patterns replaced:
//   - System.Web (line 5) — removed (Web Forms namespace)
//   - System.Web.UI (line 6) — removed (Web Forms namespace)
//   - System.Web.UI.WebControls (line 7) — removed (Web Forms namespace)
//   - public partial class userlogin : System.Web.UI.Page — replaced with PageModel
//   - Page_Load(object sender, EventArgs e) event handler — replaced with OnGet()
//   - Btn_Submit(object sender, EventArgs e) postback handler — replaced with OnPostLogin()
//   - Btn_reg(object sender, EventArgs e) postback handler — replaced with OnPostRegister()
//   - txtEmail.Text / txtPassword.Text server control access — replaced with [BindProperty]
//   - Response.Redirect("MainProfilePage.aspx") — replaced with RedirectToPage("/MainProfilePage")
//   - Response.Redirect("SignUpForm.aspx") — replaced with RedirectToPage("/SignUpForm")
//   - Server.Transfer() — removed (not applicable in ASP.NET Core)
//
// cr-dotnet-0010: Replaced Web.config / ConfigurationManager connection string lookup with
// environment variable and AWS Systems Manager Parameter Store resolution.
// Web.config transformation files (Web.Debug.config, Web.Release.config) are no longer used.
// Configuration is injected at runtime via:
//   1. RDS_CONNECTION_STRING environment variable (highest priority)
//   2. AWS SSM Parameter Store key /tour-management/dbconnection (injected as SSM_DBCONNECTION env var)
// This enables immutable deployments and true infrastructure-as-code on AWS.

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Tour_Management.Pages
{
    /// <summary>
    /// ASP.NET Core Razor Pages PageModel for the User Login page.
    /// Migrated from Web Forms userlogin.aspx / userlogin.aspx.cs (cr-dotnet-0026).
    /// cr-dotnet-0010: Configuration resolved from environment variables / AWS SSM Parameter Store
    /// instead of Web.config / ConfigurationManager (eliminates Web.config transformation dependency).
    /// </summary>
    public class UserLoginModel : PageModel
    {
        // cr-dotnet-0010: Retrieve the connection string from environment variables or
        // AWS Systems Manager Parameter Store — NOT from Web.config / ConfigurationManager.
        // Priority order:
        //   1. RDS_CONNECTION_STRING environment variable (set in ECS task definition / Elastic Beanstalk env)
        //   2. SSM_DBCONNECTION environment variable (injected from /tour-management/dbconnection SSM key)
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

        // Replaces <asp:TextBox ID="txtEmail"> server control — bound via model binding
        [BindProperty]
        public string Email { get; set; }

        // Replaces <asp:TextBox ID="txtPassword"> server control — bound via model binding
        [BindProperty]
        public string Password { get; set; }

        // Status message to display login result — replaces Response.Write()
        public string StatusMessage { get; private set; }

        // Replaces Page_Load(object sender, EventArgs e) Web Forms event handler
        public void OnGet()
        {
            // No initialization required on GET
        }

        // Replaces Btn_Submit(object sender, EventArgs e) Web Forms postback handler
        // Handles the Login button click
        public IActionResult OnPostLogin()
        {
            // Use Dapper with RDS Proxy connection string for cloud-native connection pooling.
            // SqlConnection is opened inside a using block to ensure proper disposal.
            using (var conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                string checkPasswordQuery = "select password from Userinfo where password=@Password and email=@Email";
                string storedPassword = conn.ExecuteScalar<string>(checkPasswordQuery, new
                {
                    Password = Password,
                    Email    = Email
                }) ?? "";

                if (storedPassword == Password)
                {
                    // Replaces: Response.Redirect("MainProfilePage.aspx") and Server.Transfer("MainProfilePage.aspx")
                    return RedirectToPage("/MainProfilePage");
                }
                else
                {
                    // Replaces: Response.Write("Password is not correct")
                    StatusMessage = "Password is not correct";
                    return Page();
                }
            }
        }

        // Replaces Btn_reg(object sender, EventArgs e) Web Forms postback handler
        // Handles the Sign Up button click
        public IActionResult OnPostRegister()
        {
            // Replaces: Response.Redirect("SignUpForm.aspx") and Server.Transfer("SignUpForm.aspx")
            return RedirectToPage("/SignUpForm");
        }
    }
}
