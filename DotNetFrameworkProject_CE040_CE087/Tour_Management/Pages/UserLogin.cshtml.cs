// Migrated from Web Forms (userlogin.aspx.cs) to ASP.NET Core Razor Pages (cr-dotnet-0026)
// cr-dotnet-0010: Replaced Web.config / ConfigurationManager connection string lookup with
//                 Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") so that
//                 configuration is injected at runtime (AWS ECS/EB environment variable or
//                 AWS Systems Manager Parameter Store) rather than baked into build artifacts
//                 via Web.Debug.config / Web.Release.config XDT transformations.
// Changes:
//   - Removed: using System.Web.UI;                    (line 7 - Web Forms namespace)
//   - Removed: using System.Web.UI.WebControls;        (line 8 - Web Forms namespace)
//   - Removed: inheritance from System.Web.UI.Page     (line 12 - Web Forms base class)
//   - Replaced: Page_Load event handler                -> OnGet() method in PageModel
//   - Replaced: Btn_Submit event handler               -> OnPostLoginAsync() in PageModel
//   - Replaced: Btn_reg event handler                  -> OnPostRegisterAsync() in PageModel
//   - Replaced: Response.Redirect("MainProfilePage.aspx") -> RedirectToPage("/MainProfilePage")
//   - Replaced: Response.Redirect("SignUpForm.aspx")   -> RedirectToPage("/SignUpForm")
//   - Replaced: server control property access (txtEmail.Text, txtPassword.Text)
//     with bound model properties (Input.Email, Input.Password)

using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Tour_Management.Pages
{
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

        // Retrieve the RDS Proxy connection string from environment variable,
        // falling back to the appsettings.json connectionString for local development.
        private string GetConnectionString()
        {
            return Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                ?? _configuration.GetConnectionString("dbconnection")
                ?? string.Empty;
        }

        // Replaces Web Forms Page_Load event handler
        public void OnGet()
        {
            // Initial page load — no action required
        }

        // Replaces Web Forms Btn_Submit event handler (login action)
        public async Task<IActionResult> OnPostLoginAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Use Dapper with RDS Proxy-backed SqlConnection for cloud-native connection pooling.
                using (IDbConnection conn = new SqlConnection(GetConnectionString()))
                {
                    string checkPasswordQuery = "SELECT password FROM Userinfo WHERE password=@Password AND email=@Email";
                    string password = await conn.ExecuteScalarAsync<string>(checkPasswordQuery, new
                    {
                        Password = Input.Password,
                        Email    = Input.Email
                    }) ?? "";

                    if (password == Input.Password)
                    {
                        _logger.LogInformation("User {Email} logged in successfully.", Input.Email);
                        // Replaces Response.Redirect("MainProfilePage.aspx")
                        return RedirectToPage("/MainProfilePage");
                    }
                    else
                    {
                        _logger.LogWarning("Failed login attempt for {Email}.", Input.Email);
                        StatusMessage = "Password is not correct";
                        return Page();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Email}.", Input.Email);
                StatusMessage = "Login failed. Please try again.";
                return Page();
            }
        }

        // Replaces Web Forms Btn_reg event handler (redirect to sign-up)
        public IActionResult OnPostRegisterAsync()
        {
            // Replaces Response.Redirect("SignUpForm.aspx")
            return RedirectToPage("/SignUpForm");
        }
    }

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
