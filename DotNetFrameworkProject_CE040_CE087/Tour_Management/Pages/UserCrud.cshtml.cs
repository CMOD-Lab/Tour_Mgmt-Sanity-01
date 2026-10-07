// Migrated from Web Forms (usercrud.aspx.cs) to ASP.NET Core Razor Pages (cr-dotnet-0026)
// Changes:
//   - Removed: using System.Web.UI;                    (line 5 - Web Forms namespace)
//   - Removed: using System.Web.UI.WebControls;        (line 6 - Web Forms namespace)
//   - Removed: inheritance from System.Web.UI.Page     (line 10 - Web Forms base class)
//   - Replaced: Page_Load event handler                (line 12 - Web Forms lifecycle)
//     with OnGetAsync() method in PageModel
//   - Replaced: asp:SqlDataSource declarative data binding (complex EXCEPT query)
//     with explicit Dapper query in OnGetAsync()
//   - Replaced: asp:GridView with AutoGenerateEditButton
//     with strongly-typed Razor table and OnPostEditAsync() handler
//   - Replaced: UpdateCommand on asp:SqlDataSource
//     with explicit Dapper update in OnPostEditAsync()

using System;
using System.Collections.Generic;
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
    public class UserCrudModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserCrudModel> _logger;

        public UserCrudModel(IConfiguration configuration, ILogger<UserCrudModel> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public IEnumerable<UserViewModel> Users { get; set; } = new List<UserViewModel>();
        public string StatusMessage { get; set; } = string.Empty;

        // Retrieve the RDS Proxy connection string from environment variable,
        // falling back to the appsettings.json connectionString for local development.
        private string GetConnectionString()
        {
            return Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                ?? _configuration.GetConnectionString("dbconnection")
                ?? string.Empty;
        }

        // Replaces Web Forms Page_Load event (line 12 in original code-behind)
        // and asp:SqlDataSource SelectCommand with the complex EXCEPT query
        public async Task OnGetAsync()
        {
            // Use Dapper with RDS Proxy-backed SqlConnection for cloud-native connection pooling.
            // Preserves original business logic: select top 1 record using EXCEPT pattern
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string selectQuery = @"Select top (select COUNT(*) from UserInfo) * From UserInfo
EXCEPT
Select top ((select COUNT(*) from UserInfo)-(1)) * From UserInfo";
                Users = await conn.QueryAsync<UserViewModel>(selectQuery);
            }
        }

        // Replaces AutoGenerateEditButton on asp:GridView with
        // UpdateCommand on asp:SqlDataSource
        public async Task<IActionResult> OnPostEditAsync(
            string email, string firstName, string lastName,
            string gender, string password, string city)
        {
            try
            {
                using (IDbConnection conn = new SqlConnection(GetConnectionString()))
                {
                    string updateQuery = "UPDATE [UserInfo] SET [Email]=@Email, [FirstName]=@FirstName, " +
                                        "[LastName]=@LastName, [Gender]=@Gender, [Password]=@Password, " +
                                        "[City]=@City WHERE [Email]=@Email";
                    await conn.ExecuteAsync(updateQuery, new
                    {
                        Email     = email,
                        FirstName = firstName,
                        LastName  = lastName,
                        Gender    = gender,
                        Password  = password,
                        City      = city
                    });
                }
                _logger.LogInformation("User {Email} updated successfully.", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {Email}.", email);
                StatusMessage = "Update failed. Please try again.";
            }

            return RedirectToPage();
        }
    }

    public class UserViewModel
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }
}
