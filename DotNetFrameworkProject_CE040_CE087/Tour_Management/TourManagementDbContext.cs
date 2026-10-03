using System;
using Microsoft.EntityFrameworkCore;

namespace Tour_Management
{
    /// <summary>
    /// Entity Framework Core DbContext for Tour Management application.
    ///
    /// Cloud Readiness Fix (Rule: cr-dotnet-1034):
    /// Provides async Task-based data access patterns using EF Core connected to
    /// Amazon RDS, replacing synchronous asp:SqlDataSource and asp:GridView Web Forms
    /// controls. Enables thread-pool-safe async/await operations that prevent thread
    /// pool exhaustion under load and support efficient auto-scaling in cloud deployments.
    ///
    /// Connection string is sourced from the DB_CONNECTION_STRING environment variable
    /// (cloud-native 12-factor app pattern) pointing to Amazon RDS instance.
    /// </summary>
    public class TourManagementDbContext : DbContext
    {
        public TourManagementDbContext(DbContextOptions<TourManagementDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Tours table - replaces synchronous asp:SqlDataSource SELECT FROM [Tour].
        /// Used by DisplayTours and TourCrud Razor Pages with async ToListAsync().
        /// </summary>
        public DbSet<Tour> Tours { get; set; }

        /// <summary>
        /// Bookings table - replaces synchronous asp:SqlDataSource SELECT FROM [booking].
        /// Used by AllBooking and MyBooking Razor Pages with async ToListAsync().
        /// Cloud Readiness Fix (Rule: cr-dotnet-1034): async EF Core replaces
        /// synchronous asp:GridView DataBind() and asp:SqlDataSource SelectCommand/DeleteCommand.
        /// </summary>
        public DbSet<Booking> Bookings { get; set; }

        /// <summary>
        /// UserInfo table - replaces synchronous asp:SqlDataSource SELECT/UPDATE FROM [UserInfo].
        /// Used by UserCrud Razor Page with async ToListAsync() and SaveChangesAsync().
        /// Cloud Readiness Fix (Rule: cr-dotnet-1034): async EF Core replaces
        /// synchronous asp:GridView AutoGenerateEditButton and asp:SqlDataSource
        /// SelectCommand/UpdateCommand in usercrud.aspx.
        /// </summary>
        public DbSet<UserInfo> UserInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tour entity mapping - matches original [Tour] table schema
            modelBuilder.Entity<Tour>(entity =>
            {
                entity.ToTable("Tour");
                entity.HasKey(e => e.TourId);
                entity.Property(e => e.TourId).HasColumnName("TOUR_ID");
                entity.Property(e => e.TourName).HasColumnName("TOUR_NAME").HasMaxLength(255);
                entity.Property(e => e.Place).HasColumnName("PLACE").HasMaxLength(255);
                entity.Property(e => e.Days).HasColumnName("DAYS").HasMaxLength(50);
                entity.Property(e => e.Price).HasColumnName("PRICE").HasColumnType("decimal(18,2)");
                entity.Property(e => e.Locations).HasColumnName("LOCATIONS").HasMaxLength(500);
                entity.Property(e => e.TourInfo).HasColumnName("TOUR_INFO").HasMaxLength(1000);
                entity.Property(e => e.Pic).HasColumnName("pic").HasMaxLength(500);
            });

            // Booking entity mapping - matches original [booking] table schema
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("booking");
                entity.HasKey(e => e.TourId);
                entity.Property(e => e.TourId).HasColumnName("TOUR_ID");
                entity.Property(e => e.TourName).HasColumnName("TOUR_NAME").HasMaxLength(255);
                entity.Property(e => e.Place).HasColumnName("PLACE").HasMaxLength(255);
                entity.Property(e => e.Email).HasColumnName("Email").HasMaxLength(255);
                entity.Property(e => e.FirstName).HasColumnName("FirstName").HasMaxLength(255);
            });

            // UserInfo entity mapping - matches original [UserInfo] table schema
            // Replaces synchronous asp:SqlDataSource SelectCommand/UpdateCommand in usercrud.aspx
            modelBuilder.Entity<UserInfo>(entity =>
            {
                entity.ToTable("UserInfo");
                entity.HasKey(e => e.Email);
                entity.Property(e => e.Email).HasColumnName("Email").HasMaxLength(255);
                entity.Property(e => e.FirstName).HasColumnName("FirstName").HasMaxLength(255);
                entity.Property(e => e.LastName).HasColumnName("LastName").HasMaxLength(255);
                entity.Property(e => e.Gender).HasColumnName("Gender").HasMaxLength(50);
                entity.Property(e => e.Password).HasColumnName("Password").HasMaxLength(255);
                entity.Property(e => e.City).HasColumnName("City").HasMaxLength(255);
            });
        }
    }

    /// <summary>
    /// Tour entity - maps to the [Tour] table in Amazon RDS.
    /// Replaces the synchronous asp:SqlDataSource data model used by asp:GridView.
    /// </summary>
    public class Tour
    {
        public int TourId { get; set; }
        public string TourName { get; set; }
        public string Place { get; set; }
        public string Days { get; set; }
        public decimal Price { get; set; }
        public string Locations { get; set; }
        public string TourInfo { get; set; }
        public string Pic { get; set; }
    }

    /// <summary>
    /// Booking entity - maps to the [booking] table in Amazon RDS.
    /// Replaces the synchronous asp:SqlDataSource data model used by asp:GridView.
    /// Cloud Readiness Fix (Rule: cr-dotnet-1034): async EF Core replaces synchronous
    /// asp:GridView DataBind() and asp:SqlDataSource in mybooking.aspx.
    /// </summary>
    public class Booking
    {
        public int TourId { get; set; }
        public string TourName { get; set; }
        public string Place { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
    }

    /// <summary>
    /// UserInfo entity - maps to the [UserInfo] table in Amazon RDS.
    /// Replaces the synchronous asp:SqlDataSource data model used by asp:GridView
    /// in usercrud.aspx (AutoGenerateEditButton="True", DataSourceID="SqlDataSource1").
    /// Cloud Readiness Fix (Rule: cr-dotnet-1034): async EF Core replaces synchronous
    /// asp:GridView DataBind() and asp:SqlDataSource SelectCommand/UpdateCommand.
    /// </summary>
    public class UserInfo
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Password { get; set; }
        public string City { get; set; }
    }
}
