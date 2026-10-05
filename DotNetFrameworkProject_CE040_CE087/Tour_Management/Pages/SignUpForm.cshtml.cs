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
    /// Razor Page model for user registration.
    /// Migrated from ASP.NET Web Forms (SignUpForm.aspx / SignUpForm.aspx.cs)
    /// to ASP.NET Core Razor Pages to enable cloud-native deployment and
    /// horizontal scalability on AWS.
    ///
    /// Replaces:
    ///   - System.Web.UI (line 7 in original .aspx.cs)
    ///   - System.Web.UI.WebControls (line 8 in original .aspx.cs)
    ///   - public partial class SignUpForm : System.Web.UI.Page (line 12 in original .aspx.cs)
    ///   - Page_Load / Register_Click event handlers (line 14 in original .aspx.cs)
    /// </summary>
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
            // Initialize page — no data loading required for registration form
        }

        /// <summary>
        /// Handles POST request — equivalent to Register_Click event handler in Web Forms.
        /// Inserts a new user record into the UserInfo table using Dapper with RDS Proxy-compatible
        /// connection pooling.
        /// </summary>
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Input.Password != Input.ConfirmPassword)
            {
                ModelState.AddModelError("Input.ConfirmPassword", "Passwords do not match.");
                return Page();
            }

            try
            {
                // Use Dapper with RDS Proxy-compatible SqlConnection (connection pooling handled by RDS Proxy)
                using (IDbConnection conn = new SqlConnection(GetConnectionString()))
                {
                    string insertQuery = "insert into UserInfo(Email,FirstName,LastName,Gender,Password,dob,Street,City,State) " +
                                        "values(@email,@FirstName,@LastName,@Gender,@Password,@dob,@Street,@City,@State)";
                    conn.Execute(insertQuery, new
                    {
                        email = Input.Email,
                        FirstName = Input.FirstName,
                        LastName = Input.LastName,
                        Gender = Input.Gender,
                        Password = Input.Password,
                        dob = Input.DateOfBirth,
                        Street = Input.Street,
                        City = Input.City,
                        State = Input.State
                    });
                }

                _logger.LogInformation("User registered successfully: {Email}", Input.Email);
                return RedirectToPage("/userlogin");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user: {Email}", Input.Email);
                StatusMessage = "Registration failed. Please try again.";
                IsSuccess = false;
                return Page();
            }
        }
    }

    /// <summary>
    /// Input model for the sign-up form — replaces individual server controls
    /// (TextBox, DropDownList) from the original Web Forms page.
    /// </summary>
    public class SignUpInputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string Gender { get; set; } = "Male";

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public string DateOfBirth { get; set; } = string.Empty;

        [Required]
        public string Street { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public string State { get; set; } = string.Empty;
    }
}
