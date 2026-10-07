// cr-dotnet-1034: Async GridView Data Binding with RDS via Entity Framework Core
// Replaced synchronous OnGet() / RefreshData() / OnPostUpdate() with async Task-based handlers
// using Entity Framework Core connected to Amazon RDS, preventing thread pool exhaustion under load.
//
// MIGRATION NOTE (cr-dotnet-0026 - Web Forms Usage):
// This file has been migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
//
// Original Web Forms code-behind:
//   - Inherited from System.Web.UI.Page (line 10) — replaced with PageModel (ASP.NET Core Razor Pages)
//   - Used System.Web (line 4) — removed (Web Forms namespace)
//   - Used System.Web.UI (line 5) — removed (Web Forms namespace)
//   - Used System.Web.UI.WebControls (line 6) — removed (Web Forms namespace)
//   - Page_Load(object sender, EventArgs e) event handler — replaced with OnGetAsync() Razor Page handler
//   - <asp:GridView> DataSource / DataBind() — replaced with Users property populated via EF Core
//   - <asp:SqlDataSource> declarative SelectCommand / UpdateCommand — replaced with async OnGetAsync() / OnPostUpdateAsync()
//
// cr-dotnet-1034 changes:
//   - Replaced: synchronous void OnGet() with async Task OnGetAsync()
//   - Replaced: synchronous RefreshData() with async RefreshDataAsync() using EF Core ToListAsync()
//   - Replaced: synchronous IActionResult OnPostUpdate() with async Task<IActionResult> OnPostUpdateAsync()
//   - Replaced: Dapper SqlConnection with EF Core UserCrudDbContext (DbContext)
//   - Connection string retrieved from RDS_CONNECTION_STRING environment variable

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Tour_Management.Pages
{
    /// <summary>
    /// ASP.NET Core Razor Pages PageModel for the User CRUD admin page.
    /// Migrated from Web Forms usercrud.aspx / usercrud.aspx.cs (cr-dotnet-0026).
    /// cr-dotnet-1034: All data access converted to async EF Core patterns for Amazon RDS.
    /// </summary>
    public class UserCrudModel : PageModel
    {
        // Retrieve the RDS connection string from environment variable (Amazon RDS / RDS Proxy endpoint)
        // or fall back to appsettings.json / environment connectionStrings entry "dbconnection".
        private static string GetConnectionString()
        {
            string envConnStr = Environment.GetEnvironmentVariable("RDS_CONNECTION_STRING");
            if (!string.IsNullOrEmpty(envConnStr))
                return envConnStr;
            return System.Configuration.ConfigurationManager.ConnectionStrings["dbconnection"]?.ConnectionString
                ?? string.Empty;
        }

        // Replaces <asp:GridView> DataSource — populated in OnGetAsync() and rendered via @foreach in the view
        public IEnumerable<UserInfoEntity> Users { get; private set; } = new List<UserInfoEntity>();

        // cr-dotnet-1034: Replaced synchronous void OnGet() with async Task OnGetAsync()
        // Prevents thread pool exhaustion under cloud load
        public async Task OnGetAsync()
        {
            await RefreshDataAsync();
        }

        // cr-dotnet-1034: Replaced synchronous RefreshData() with async RefreshDataAsync() using EF Core
        // Replaces the implicit data binding of <asp:SqlDataSource> SelectCommand to GridView1
        private async Task RefreshDataAsync()
        {
            var optionsBuilder = new DbContextOptionsBuilder<UserCrudDbContext>();
            optionsBuilder.UseSqlServer(GetConnectionString());

            using (var dbContext = new UserCrudDbContext(optionsBuilder.Options))
            {
                // Async EF Core query — prevents thread pool exhaustion under cloud load
                // Replaces: SelectCommand="Select top (select COUNT(*) from UserInfo) * From UserInfo
                //            EXCEPT Select top ((select COUNT(*) from UserInfo)-(1)) * From UserInfo"
                Users = await dbContext.Users
                    .AsNoTracking()
                    .ToListAsync();
            }
        }

        // cr-dotnet-1034: Replaced synchronous IActionResult OnPostUpdate() with async Task<IActionResult> OnPostUpdateAsync()
        // Replaces <asp:SqlDataSource> UpdateCommand handler (AutoGenerateEditButton postback)
        // Equivalent to: UPDATE [UserInfo] Set [Email]=@Email,[FirstName]=@FirstName,[LastName]=@LastName,
        //                [Gender]=@Gender,[Password]=@Password,[City]=@City Where [Email]=@Email
        public async Task<IActionResult> OnPostUpdateAsync(string email, string firstName, string lastName,
                                                            string gender, string password, string city)
        {
            var optionsBuilder = new DbContextOptionsBuilder<UserCrudDbContext>();
            optionsBuilder.UseSqlServer(GetConnectionString());

            using (var dbContext = new UserCrudDbContext(optionsBuilder.Options))
            {
                var user = await dbContext.Users.FindAsync(email);
                if (user != null)
                {
                    user.FirstName = firstName;
                    user.LastName  = lastName;
                    user.Gender    = gender;
                    user.Password  = password;
                    user.City      = city;
                    await dbContext.SaveChangesAsync();
                }
            }
            return RedirectToPage();
        }
    }

    // EF Core entity class mapping to the [UserInfo] table in Amazon RDS
    [System.ComponentModel.DataAnnotations.Schema.Table("UserInfo")]
    public class UserInfoEntity
    {
        [System.ComponentModel.DataAnnotations.Key]
        [System.ComponentModel.DataAnnotations.Schema.Column("Email")]
        public string Email { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Schema.Column("FirstName")]
        public string FirstName { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Schema.Column("LastName")]
        public string LastName { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Schema.Column("Gender")]
        public string Gender { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Schema.Column("Password")]
        public string Password { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Schema.Column("City")]
        public string City { get; set; } = string.Empty;
    }

    // EF Core DbContext for User CRUD — connects to Amazon RDS
    public class UserCrudDbContext : DbContext
    {
        public UserCrudDbContext(DbContextOptions<UserCrudDbContext> options) : base(options) { }

        public DbSet<UserInfoEntity> Users { get; set; }
    }
}
