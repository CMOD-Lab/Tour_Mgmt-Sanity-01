using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Tour_Management.Pages
{
    /// <summary>
    /// Razor Page model for user login.
    /// Migrated from ASP.NET Web Forms (userlogin.aspx / userlogin.aspx.cs)
    /// to ASP.NET Core Razor Pages to enable cloud-native deployment and
    /// horizontal scalability on AWS.
    ///
    /// Replaces:
    ///   - using System.Web;                (line 7 in original userlogin.aspx.cs)
    ///   - using System.Web.UI;             (line 8 in original userlogin.aspx.cs)
    ///   - using System.Web.UI.WebControls; (line 9 in original userlogin.aspx.cs)
    ///   - public partial class userlogin : System.Web.UI.Page  (line 12 in original)
    ///   - Btn_Submit / Btn_reg event handlers (original userlogin.aspx.cs)
    ///   - asp:Label, asp:TextBox, asp:Button server controls (original userlogin.aspx)
    ///   - <%@ Page ... %> directive        (line 1 in original userlogin.aspx)
    /// </summary>
    public class UserLoginModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserLoginModel> _logger;

        public UserLoginModel(IConfiguration configuration, ILogger<UserLoginModel> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [BindProperty]
        public LoginInputModel Input { get; set; } = new LoginInputModel();

        public string StatusMessage { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Retrieves the database connection string from environment variable (AWS RDS Proxy endpoint)
        /// with fallback to appsettings.json for local development.
        /// </summary>
        private string GetConnectionString()
        {
            string envConnStr = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            if (!string.IsNullOrEmpty(envConnStr))
                return envConnStr;
            return _configuration.GetConnectionString("dbconnection");
        }

        /// <summary>
        /// Handles GET request — equivalent to Page_Load (non-postback) in Web Forms.
        /// </summary>
        public void OnGet()
        {
            // Initialize page — no data loading required for login form
        }

        /// <summary>
        /// Handles POST for Login — equivalent to Btn_Submit event handler in Web Forms.
        /// Validates user credentials using Dapper with RDS Proxy-compatible connection pooling.
        /// Replaces: select password from Userinfo where password=@Password and email=@Email
        /// On success redirects to MainProfilePage (equivalent to Response.Redirect("MainProfilePage.aspx")).
        /// </summary>
        public IActionResult OnPostLogin()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Use Dapper with RDS Proxy-compatible SqlConnection (connection pooling handled by RDS Proxy)
                string password = null;
                using (IDbConnection conn = new SqlConnection(GetConnectionString()))
                {
                    string checkPasswordQuery = "select password from Userinfo where password=@Password and email=@Email";
                    password = conn.ExecuteScalar<string>(checkPasswordQuery, new
                    {
                        Password = Input.Password,
                        Email = Input.Email
                    }) ?? string.Empty;
                }

                if (password == Input.Password)
                {
                    _logger.LogInformation("User logged in successfully: {Email}", Input.Email);
                    return RedirectToPage("/MainProfilePage");
                }
                else
                {
                    _logger.LogWarning("Failed login attempt for: {Email}", Input.Email);
                    StatusMessage = "Password is not correct";
                    IsSuccess = false;
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for: {Email}", Input.Email);
                StatusMessage = "Login failed. Please try again.";
                IsSuccess = false;
                return Page();
            }
        }

        /// <summary>
        /// Handles POST for Register redirect — equivalent to Btn_reg event handler in Web Forms.
        /// Redirects to the SignUpForm page (equivalent to Response.Redirect("SignUpForm.aspx")).
        /// </summary>
        public IActionResult OnPostRegister()
        {
            return RedirectToPage("/SignUpForm");
        }
    }

    /// <summary>
    /// Input model for the login form — replaces individual server controls
    /// (asp:TextBox for email and password) from the original Web Forms page.
    /// </summary>
    public class LoginInputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
