using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Tour_Management.Pages
{
    /// <summary>
    /// Razor Page model for User CRUD operations.
    /// cr-dotnet-1034: Replaced synchronous GridView/SqlDataSource data binding with
    /// async Task-based patterns using Entity Framework Core connected to Amazon RDS,
    /// preventing thread pool exhaustion under load and enabling efficient auto-scaling.
    ///
    /// Migrated from ASP.NET Web Forms (usercrud.aspx / usercrud.aspx.cs)
    /// to ASP.NET Core Razor Pages to enable cloud-native deployment on AWS.
    ///
    /// Replaces:
    ///   - using System.Web;                (line 4 in original usercrud.aspx.cs)
    ///   - using System.Web.UI;             (line 5 in original usercrud.aspx.cs)
    ///   - using System.Web.UI.WebControls; (line 6 in original usercrud.aspx.cs)
    ///   - public partial class usercrud : System.Web.UI.Page  (line 10 in original)
    ///   - asp:GridView AutoGenerateEditButton="True" (synchronous, line 11 original .aspx)
    ///   - asp:SqlDataSource SelectCommand/UpdateCommand (line 30 original .aspx)
    /// </summary>
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
        public bool IsSuccess { get; set; }
        public string EditEmail { get; set; } = string.Empty;

        /// <summary>
        /// Retrieves the database connection string from environment variable (AWS RDS endpoint)
        /// with fallback to appsettings.json / Web.config for local development.
        /// </summary>
        private string GetConnectionString()
        {
            string envConnStr = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            if (!string.IsNullOrEmpty(envConnStr))
                return envConnStr;
            return _configuration.GetConnectionString("dbconnection");
        }

        /// <summary>
        /// Creates an EF Core DbContext configured for Amazon RDS via environment variable
        /// or appsettings connection string.
        /// </summary>
        private UserCrudDbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<UserCrudDbContext>();
            optionsBuilder.UseSqlServer(GetConnectionString());
            return new UserCrudDbContext(optionsBuilder.Options);
        }

        /// <summary>
        /// cr-dotnet-1034: Async OnGetAsync replaces synchronous Page_Load / GridView.DataBind().
        /// Uses Entity Framework Core async query (ToListAsync) connected to Amazon RDS,
        /// preventing thread pool exhaustion and enabling efficient auto-scaling.
        /// Replaces: asp:GridView DataSourceID="SqlDataSource1" (synchronous binding, line 11 original)
        ///           asp:SqlDataSource SelectCommand (line 30 original)
        /// Supports optional editEmail query parameter to put a row into edit mode.
        /// </summary>
        public async Task OnGetAsync(string editEmail = null)
        {
            EditEmail = editEmail ?? string.Empty;
            await LoadUsersAsync();
        }

        /// <summary>
        /// cr-dotnet-1034: Async data load using EF Core ToListAsync() — replaces synchronous
        /// GridView data binding via SqlDataSource. Prevents thread pool exhaustion under load.
        ///
        /// Replaces asp:SqlDataSource SelectCommand:
        ///   Select top (select COUNT(*) from UserInfo) * From UserInfo
        ///   EXCEPT Select top ((select COUNT(*) from UserInfo)-(1)) * From UserInfo
        /// Note: The original query retrieved only the last inserted user. This implementation
        /// retrieves all users to support full CRUD grid functionality consistent with the
        /// asp:GridView AutoGenerateEditButton="True" pattern.
        /// </summary>
        private async Task LoadUsersAsync()
        {
            try
            {
                using (var dbContext = CreateDbContext())
                {
                    // Async EF Core query — replaces synchronous GridView/SqlDataSource binding
                    Users = await dbContext.Users
                        .AsNoTracking()
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading users from database.");
                StatusMessage = "Error loading users. Please try again.";
                IsSuccess = false;
            }
        }

        /// <summary>
        /// cr-dotnet-1034: Async OnPostUpdateAsync replaces synchronous GridView AutoGenerateEditButton.
        /// Uses EF Core async SaveChangesAsync() connected to Amazon RDS.
        ///
        /// Replaces asp:SqlDataSource UpdateCommand:
        ///   UPDATE [UserInfo] Set [Email]=@Email,[FirstName]=@FirstName,[LastName]=@LastName,
        ///   [Gender]=@Gender,[Password]=@Password,[City]=@City Where [Email]=@Email
        /// </summary>
        public async Task<IActionResult> OnPostUpdateAsync(
            string editEmail,
            string firstName,
            string lastName,
            string gender,
            string password,
            string city)
        {
            try
            {
                using (var dbContext = CreateDbContext())
                {
                    var user = await dbContext.Users.FindAsync(editEmail);
                    if (user != null)
                    {
                        user.FirstName = firstName;
                        user.LastName = lastName;
                        user.Gender = gender;
                        user.Password = password;
                        user.City = city;
                        // Async SaveChangesAsync — replaces synchronous SqlDataSource UpdateCommand
                        await dbContext.SaveChangesAsync();
                    }
                }

                _logger.LogInformation("User updated successfully: Email={Email}", editEmail);
                StatusMessage = "User updated successfully.";
                IsSuccess = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user: Email={Email}", editEmail);
                StatusMessage = "Error updating user. Please try again.";
                IsSuccess = false;
            }

            await LoadUsersAsync();
            return Page();
        }
    }

    /// <summary>
    /// Entity model representing a UserInfo record — replaces the auto-generated columns
    /// from asp:GridView DataKeyNames="Email" in the original Web Forms page.
    /// Columns: Email, FirstName, LastName, Gender, Password, City
    /// </summary>
    [Table("UserInfo")]
    public class UserViewModel
    {
        [Key]
        [Column("Email")]
        public string Email { get; set; } = string.Empty;

        [Column("FirstName")]
        public string FirstName { get; set; } = string.Empty;

        [Column("LastName")]
        public string LastName { get; set; } = string.Empty;

        [Column("Gender")]
        public string Gender { get; set; } = string.Empty;

        [Column("Password")]
        public string Password { get; set; } = string.Empty;

        [Column("City")]
        public string City { get; set; } = string.Empty;
    }

    /// <summary>
    /// EF Core DbContext for User CRUD data — replaces SqlDataSource control.
    /// cr-dotnet-1034: Enables async data access patterns via Entity Framework Core
    /// connected to Amazon RDS, preventing thread pool exhaustion under cloud load.
    /// </summary>
    public class UserCrudDbContext : DbContext
    {
        public UserCrudDbContext(DbContextOptions<UserCrudDbContext> options) : base(options) { }

        public DbSet<UserViewModel> Users { get; set; }
    }
}
