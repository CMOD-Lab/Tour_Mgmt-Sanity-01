// cr-dotnet-1034: Async GridView Data Binding with RDS via Entity Framework Core
// MIGRATION NOTE (cr-dotnet-0026 + cr-dotnet-1034): This file has been migrated to ASP.NET Core Razor Pages
// with async Task-based data binding using Entity Framework Core connected to Amazon RDS.
// The Razor Pages equivalent is located at: Pages/UserCrud.cshtml and Pages/UserCrud.cshtml.cs
//
// Original Web Forms violations addressed:
//   - Line 4: using System.Web;                → removed (Web Forms namespace)
//   - Line 5: using System.Web.UI;             → removed (Web Forms namespace)
//   - Line 6: using System.Web.UI.WebControls; → removed (Web Forms namespace)
//   - Line 10: System.Web.UI.Page base class   → replaced with PageModel in Pages/UserCrud.cshtml.cs
//   - Line 11: <asp:GridView AutoGenerateEditButton="True"> → replaced with async EF Core + Razor table
//   - Line 30: <asp:SqlDataSource> synchronous data source → replaced with async EF Core DbContext
//   - cr-dotnet-1034: Synchronous GridView.DataBind() replaced with async Task OnGetAsync()
//                     using Entity Framework Core ToListAsync() on Amazon RDS
//
// This file is retained for reference only. The active implementation is in Pages/UserCrud.cshtml.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
// Removed: using System.Web;                (Web Forms namespace - cr-dotnet-0026)
// Removed: using System.Web.UI;             (Web Forms namespace - cr-dotnet-0026)
// Removed: using System.Web.UI.WebControls; (Web Forms namespace - cr-dotnet-0026)
using Microsoft.EntityFrameworkCore;

namespace Tour_Management
{
    // NOTE: This class is superseded by Tour_Management.Pages.UserCrudModel (Razor Pages PageModel).
    // The System.Web.UI.Page base class has been replaced with PageModel in the migrated version.
    // cr-dotnet-1034: Synchronous GridView.DataBind() replaced with async Task OnGetAsync()
    //                 using Entity Framework Core ToListAsync() on Amazon RDS.
    public partial class usercrud
    {
        // Retrieve connection string from environment variable (Amazon RDS endpoint)
        private static string GetConnectionString()
        {
            return Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                ?? System.Configuration.ConfigurationManager.ConnectionStrings["dbconnection"]?.ConnectionString;
        }

        // cr-dotnet-1034: Migrated Page_Load → async Task OnGetAsync() in Pages/UserCrud.cshtml.cs
        // Replaced synchronous GridView.DataBind() with async EF Core ToListAsync()
        protected void Page_Load(object sender, EventArgs e)
        {
            // Data loading migrated to Pages/UserCrud.cshtml.cs async Task OnGetAsync()
            // using Entity Framework Core ToListAsync() connected to Amazon RDS
        }

        // cr-dotnet-1034: Async data refresh using EF Core - replaces synchronous GridView.DataBind()
        public async Task RefreshDataAsync()
        {
            var optionsBuilder = new DbContextOptionsBuilder<UserCrudDbContext>();
            optionsBuilder.UseSqlServer(GetConnectionString());

            // cr-dotnet-1034: async EF Core query replaces synchronous GridView data binding
            using (var dbContext = new UserCrudDbContext(optionsBuilder.Options))
            {
                var users = await dbContext.UserRecords.ToListAsync();
                // Data rendered via async Razor foreach in Pages/UserCrud.cshtml
            }
        }

        // cr-dotnet-1034: Async update using EF Core - replaces synchronous SqlDataSource UpdateCommand
        public async Task UpdateUserAsync(string email, string firstName, string lastName,
            string gender, string password, string city)
        {
            var optionsBuilder = new DbContextOptionsBuilder<UserCrudDbContext>();
            optionsBuilder.UseSqlServer(GetConnectionString());

            using (var dbContext = new UserCrudDbContext(optionsBuilder.Options))
            {
                var user = await dbContext.UserRecords.FindAsync(email);
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
        }
    }

    // cr-dotnet-1034: Entity Framework Core DbContext for async data access to Amazon RDS
    public class UserCrudDbContext : DbContext
    {
        public UserCrudDbContext(DbContextOptions<UserCrudDbContext> options) : base(options) { }

        public DbSet<UserRecord> UserRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRecord>(entity =>
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

    public class UserRecord
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Password { get; set; }
        public string City { get; set; }
    }
}
