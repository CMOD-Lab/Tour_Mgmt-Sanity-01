using System.Data.Entity;

namespace Tour_Management.Data
{
    /// <summary>
    /// Entity Framework 6 DbContext for Tour Management application.
    /// cr-dotnet-1034: Replaces synchronous SqlDataSource + GridView data binding
    /// with async Task-based EF Core patterns connected to Amazon RDS.
    ///
    /// Connection string is resolved from the DB_CONNECTION_STRING environment variable
    /// (set by AWS Parameter Store / Secrets Manager at runtime) or falls back to
    /// the Web.config "dbconnection" connection string.
    /// </summary>
    public class TourManagementDbContext : DbContext
    {
        public TourManagementDbContext()
            : base(GetConnectionString())
        {
        }

        /// <summary>
        /// Resolves the connection string from environment variable (cloud) or Web.config (local).
        /// </summary>
        private static string GetConnectionString()
        {
            return System.Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                ?? System.Configuration.ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString;
        }

        /// <summary>
        /// Tour entities — replaces synchronous SqlDataSource SELECT FROM [Tour].
        /// </summary>
        public DbSet<TourEntity> Tours { get; set; }

        /// <summary>
        /// Booking entities — replaces synchronous SqlDataSource SELECT FROM [booking].
        /// </summary>
        public DbSet<BookingEntity> Bookings { get; set; }

        /// <summary>
        /// UserInfo entities — replaces synchronous SqlDataSource SELECT FROM [UserInfo].
        /// </summary>
        public DbSet<UserInfoEntity> UserInfos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TourEntity>().ToTable("Tour");
            modelBuilder.Entity<BookingEntity>().ToTable("booking");
            modelBuilder.Entity<UserInfoEntity>().ToTable("UserInfo");
            base.OnModelCreating(modelBuilder);
        }
    }
}
