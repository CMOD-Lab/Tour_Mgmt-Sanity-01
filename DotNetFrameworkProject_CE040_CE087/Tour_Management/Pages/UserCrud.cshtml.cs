// cr-dotnet-1034: Async GridView Data Binding with RDS via Entity Framework Core
// MIGRATION NOTE (cr-dotnet-0026 + cr-dotnet-1034): ASP.NET Core Razor Pages PageModel for UserCrud page.
// Migrated from ASP.NET Web Forms (usercrud.aspx / usercrud.aspx.cs) to ASP.NET Core Razor Pages.
// - Removed: System.Web (line 4 - Web Forms namespace - cr-dotnet-0026)
// - Removed: System.Web.UI (line 5 - Web Forms namespace - cr-dotnet-0026)
// - Removed: System.Web.UI.WebControls (line 6 - Web Forms namespace - cr-dotnet-0026)
// - Removed: System.Web.UI.Page base class (line 10 - cr-dotnet-0026)
// - Replaced: System.Web.UI.Page base class with PageModel
// - Replaced: Page_Load event with async Task OnGetAsync() Razor Pages lifecycle method
// - cr-dotnet-1034: Replaced synchronous <asp:GridView AutoGenerateEditButton="True"> + <asp:SqlDataSource>
//                   with async Entity Framework Core ToListAsync() + Razor table
// - Replaced: SqlDataSource UpdateCommand with async OnPostUpdateAsync() handler using EF Core SaveChangesAsync()
// - Replaced: SqlDataSource ConnectionString binding with environment variable DB_CONNECTION_STRING (Amazon RDS)
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Tour_Management.Pages
{
    /// <summary>
    /// Razor Pages PageModel for the User CRUD management page.
    /// cr-dotnet-1034: Uses async Task OnGetAsync() with Entity Framework Core ToListAsync()
    /// connected to Amazon RDS, preventing thread pool exhaustion under cloud load.
    /// </summary>
    public class UserCrudModel : PageModel
    {
        /// <summary>
        /// Represents a single user record from the database.
        /// </summary>
        public class UserRecord
        {
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Gender { get; set; }
            public string Password { get; set; }
            public string City { get; set; }
        }

        /// <summary>
        /// List of user records loaded from Amazon RDS via async EF Core query.
        /// cr-dotnet-1034: Replaces synchronous <asp:GridView> + <asp:SqlDataSource> server controls.
        /// </summary>
        public List<UserRecord> Users { get; private set; } = new List<UserRecord>();

        // Retrieve connection string from environment variable (Amazon RDS Proxy endpoint) with fallback to configuration
        private static string GetConnectionString()
        {
            return Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                ?? System.Configuration.ConfigurationManager.ConnectionStrings["dbconnection"]?.ConnectionString;
        }

        private DbContextOptions<UserCrudPageDbContext> BuildDbOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder<UserCrudPageDbContext>();
            optionsBuilder.UseSqlServer(GetConnectionString());
            return optionsBuilder.Options;
        }

        // cr-dotnet-1034: Replaced synchronous OnGet() + GridView.DataBind() with async Task OnGetAsync()
        // Uses Entity Framework Core ToListAsync() for non-blocking Amazon RDS data access
        public async Task OnGetAsync()
        {
            // cr-dotnet-1034: Replaces synchronous <asp:SqlDataSource SelectCommand="Select top ... * From UserInfo EXCEPT ...">
            // with async EF Core ToListAsync() - prevents thread pool exhaustion under AWS cloud load
            using (var dbContext = new UserCrudPageDbContext(BuildDbOptions()))
            {
                Users = await dbContext.UserRecords.ToListAsync();
            }
        }

        // cr-dotnet-1034: Async OnPostUpdateAsync replaces synchronous AutoGenerateEditButton update action
        // Replaces: UpdateCommand="UPDATE [UserInfo] Set [Email]=@Email,[FirstName]=@FirstName,
        //           [LastName]=@LastName,[Gender]=@Gender,[Password]=@Password,[City]=@City Where [Email]=@Email"
        public async Task<IActionResult> OnPostUpdateAsync(string originalEmail, string firstName,
            string lastName, string gender, string password, string city)
        {
            using (var dbContext = new UserCrudPageDbContext(BuildDbOptions()))
            {
                var user = await dbContext.UserRecords.FindAsync(originalEmail);
                if (user != null)
                {
                    user.FirstName = firstName;
                    user.LastName = lastName;
                    user.Gender = gender;
                    user.Password = password;
                    user.City = city;
                    await dbContext.SaveChangesAsync();
                }
            }
            return RedirectToPage();
        }
    }

    // cr-dotnet-1034: Entity Framework Core DbContext for async data access to Amazon RDS
    public class UserCrudPageDbContext : DbContext
    {
        public UserCrudPageDbContext(DbContextOptions<UserCrudPageDbContext> options) : base(options) { }

        public DbSet<UserCrudModel.UserRecord> UserRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserCrudModel.UserRecord>(entity =>
            {
                entity.ToTable("UserInfo");
                entity.HasKey(e => e.Email);
                entity.Property(e => e.Email).HasColumnName("Email");
                entity.Property(e => e.FirstName).HasColumnName("FirstName");
                entity.Property(e => e.LastName).HasColumnName("LastName");
                entity.Property(e => e.Gender).HasColumnName("Gender");
                entity.Property(e => e.Password).HasColumnName("Password");
                entity.Property(e => e.City).HasColumnName("City");
            });
        }
    }
}
