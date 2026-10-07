// Migrated from Web Forms (SignUpForm.aspx.cs) to ASP.NET Core Razor Pages (cr-dotnet-0026)
// cr-dotnet-0010: Replaced Web.config / ConfigurationManager connection string lookup with
//                 Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") so that
//                 configuration is injected at runtime (AWS ECS/EB environment variable or
//                 AWS Systems Manager Parameter Store) rather than baked into build artifacts
//                 via Web.Debug.config / Web.Release.config XDT transformations.
// Changes:
//   - Removed: using System.Web.UI;                    (line 7 - Web Forms namespace)
//   - Removed: using System.Web.UI.WebControls;        (line 8 - Web Forms namespace)
//   - Removed: inheritance from System.Web.UI.Page     (line 12 - Web Forms base class)
//   - Replaced: Page_Load event handler                (line 14 - Web Forms lifecycle)
//     with OnGet() method in PageModel
//   - Replaced: Register_Click event handler with OnPostAsync() in PageModel
//   - Replaced: Response.Redirect() with RedirectToPage()
//   - Replaced: server control property access (email.Text) with bound model properties

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
    public class SignUpFormModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SignUpFormModel> _logger;

        public SignUpFormModel(IConfiguration configuration, ILogger<SignUpFormModel> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [BindProperty]
        public SignUpInputModel Input { get; set; } = new SignUpInputModel();

        public string StatusMessage { get; set; } = string.Empty;

        // Retrieve the RDS Proxy connection string from environment variable,
        // falling back to the appsettings.json connectionString for local development.
        private string GetConnectionString()
        {
            return Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                ?? _configuration.GetConnectionString("dbconnection")
                ?? string.Empty;
        }

        // Replaces Web Forms Page_Load event (line 14 in original code-behind)
        public void OnGet()
        {
            // Initial page load — no action required
        }

        // Replaces Web Forms Register_Click event handler
        public async Task<IActionResult> OnPostAsync()
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
                    string insertQuery = "insert into UserInfo(Email,FirstName,LastName,Gender,Password,dob,Street,City,State) " +
                                        "values(@email,@FirstName,@LastName,@Gender,@Password,@dob,@Street,@City,@State)";

                    await conn.ExecuteAsync(insertQuery, new
                    {
                        email     = Input.Email,
                        FirstName = Input.FirstName,
                        LastName  = Input.LastName,
                        Gender    = Input.Gender,
                        Password  = Input.Password,
                        dob       = Input.DateOfBirth,
                        Street    = Input.Street,
                        City      = Input.City,
                        State     = Input.State
                    });
                }

                _logger.LogInformation("User {Email} registered successfully.", Input.Email);
                // Replaces Response.Redirect("userlogin.aspx")
                return RedirectToPage("/UserLogin");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration for {Email}.", Input.Email);
                StatusMessage = "Registration failed. Please try again.";
                return Page();
            }
        }
    }

    public class SignUpInputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string Gender { get; set; } = "Male";

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public string DateOfBirth { get; set; } = string.Empty;

        [Required]
        public string Street { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public string State { get; set; } = string.Empty;
    }
}
